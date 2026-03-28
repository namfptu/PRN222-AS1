using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly SalesManagementDbContext _context;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Customer> _customerRepository;

        public OrderService(
            SalesManagementDbContext context,
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Customer> customerRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.CreatedByAccount)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> CreateOrderAsync(Order order, List<OrderDetail> details)
        {
            if (details == null || !details.Any())
            {
                throw new InvalidOperationException("Vui long chon it nhat 1 san pham.");
            }

            if (!order.CustomerId.HasValue)
            {
                throw new InvalidOperationException("Vui long chon khach hang.");
            }

            var customer = await _customerRepository.GetByIdAsync(order.CustomerId.Value);
            if (customer == null || !customer.Status)
            {
                throw new InvalidOperationException("Khach hang khong hop le.");
            }

            var productIds = details.Select(d => d.ProductId).Distinct().ToList();
            var products = await _productRepository.GetQueryable()
                .Where(p => productIds.Contains(p.Id) && p.Status)
                .ToListAsync();

            if (products.Count != productIds.Count)
            {
                throw new InvalidOperationException("Co san pham khong con ton tai hoac da ngung ban.");
            }

            foreach (var detail in details)
            {
                var product = products.First(p => p.Id == detail.ProductId);

                if (detail.Quantity <= 0)
                {
                    throw new InvalidOperationException($"So luong cua san pham '{product.Name}' khong hop le.");
                }

                if (detail.Quantity > product.Quantity)
                {
                    throw new InvalidOperationException($"San pham '{product.Name}' khong du ton kho.");
                }

                // Luon lay don gia tu server de khong bi sua tong tien tu phia client.
                detail.UnitPrice = product.Price;
            }

            order.Code = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            order.CreatedDate = DateTime.Now;
            order.Status = OrderStatus.Pending;
            order.TotalAmount = details.Sum(d => d.UnitPrice * d.Quantity);
            // Luu snapshot thong tin khach ngay tren order de hoa don cu van dung du lieu tai thoi diem ban.
            order.CustomerName = customer.FullName;
            order.CustomerPhone = customer.Phone;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Theo luong Sales hien tai, vua tao Pending la tru kho ngay.
                foreach (var detail in details)
                {
                    var product = products.First(p => p.Id == detail.ProductId);
                    product.Quantity -= detail.Quantity;
                    _productRepository.Update(product);
                }

                await _productRepository.SaveChangesAsync();

                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangesAsync();

                foreach (var detail in details)
                {
                    detail.OrderId = order.Id;
                    await _orderDetailRepository.AddAsync(detail);
                }

                await _orderDetailRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.CreatedByAccount)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            if (newStatus != OrderStatus.Done && newStatus != OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Trang thai cap nhat khong hop le.");
            }

            var order = await _orderRepository.GetQueryable()
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new InvalidOperationException("Khong tim thay don hang.");
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Chi don hang Pending moi duoc cap nhat trang thai.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (newStatus == OrderStatus.Cancelled)
                {
                    // Neu huy don thi cong tra lai kho vi so luong da bi tru ngay luc tao Pending.
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = detail.Product ?? await _productRepository.GetByIdAsync(detail.ProductId);
                        if (product == null)
                        {
                            throw new InvalidOperationException("Khong tim thay san pham trong don hang.");
                        }

                        product.Quantity += detail.Quantity;
                        _productRepository.Update(product);
                    }

                    await _productRepository.SaveChangesAsync();
                }

                // Done luc nay chi co y nghia chot trang thai vi kho da xu ly tu luc tao don.
                order.Status = newStatus;
                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
