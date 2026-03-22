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
                throw new InvalidOperationException("Vui lòng chọn ít nhất 1 sản phẩm.");
            }

            if (!order.CustomerId.HasValue)
            {
                throw new InvalidOperationException("Vui lòng chọn khách hàng.");
            }

            var customer = await _customerRepository.GetByIdAsync(order.CustomerId.Value);
            if (customer == null || !customer.Status)
            {
                throw new InvalidOperationException("Khách hàng không hợp lệ.");
            }

            var productIds = details.Select(d => d.ProductId).Distinct().ToList();
            var products = await _productRepository.GetQueryable()
                .Where(p => productIds.Contains(p.Id) && p.Status)
                .ToListAsync();

            if (products.Count != productIds.Count)
            {
                throw new InvalidOperationException("Có sản phẩm không còn tồn tại hoặc đã ngừng bán.");
            }

            foreach (var detail in details)
            {
                var product = products.First(p => p.Id == detail.ProductId);

                if (detail.Quantity <= 0)
                {
                    throw new InvalidOperationException($"Số lượng của sản phẩm '{product.Name}' không hợp lệ.");
                }

                if (detail.Quantity > product.Quantity)
                {
                    throw new InvalidOperationException($"Sản phẩm '{product.Name}' không đủ tồn kho.");
                }

                detail.UnitPrice = product.Price;
            }

            order.Code = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            order.CreatedDate = DateTime.Now;
            order.Status = OrderStatus.Pending;
            order.TotalAmount = details.Sum(d => d.UnitPrice * d.Quantity);
            order.CustomerName = customer.FullName;
            order.CustomerPhone = customer.Phone;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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
    }
}
