using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SalesManagement.Service.Implementations
{
    public class ImportOrderService : IImportOrderService
    {
        private readonly IGenericRepository<ImportOrder> _importOrderRepo;
        private readonly IGenericRepository<ImportOrderDetail> _importOrderDetailRepo;
        private readonly IGenericRepository<Product> _productRepo;

        public ImportOrderService(
            IGenericRepository<ImportOrder> importOrderRepo,
            IGenericRepository<ImportOrderDetail> importOrderDetailRepo,
            IGenericRepository<Product> productRepo)
        {
            _importOrderRepo = importOrderRepo;
            _importOrderDetailRepo = importOrderDetailRepo;
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<ImportOrder>> GetAllImportOrdersAsync()
        {
            return await _importOrderRepo.GetAllAsync();
        }

        public async Task<bool> CreateImportOrderAsync(ImportOrder order, List<ImportOrderDetail> details)
        {
            // 1. Tự động tính tổng tiền (TotalCost) dựa trên số lượng và giá vốn nhập vào
            order.TotalCost = details.Sum(d => d.Quantity * d.UnitCost);
            order.ImportDate = DateTime.Now;
            order.Status = ImportOrderStatus.Completed;

            // Random một mã phiếu nhập (Code)
            order.Code = "IMP" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // 2. Lưu phiếu nhập cha trước để lấy Id
            await _importOrderRepo.AddAsync(order);
            await _importOrderRepo.SaveChangesAsync();

            // 3. Vòng lặp: Lưu từng dòng chi tiết & CẬP NHẬT TỒN KHO
            foreach (var detail in details)
            {
                // Gắn khóa ngoại từ phiếu cha sang phiếu con
                detail.ImportOrderId = order.Id;
                await _importOrderDetailRepo.AddAsync(detail);

                // Nghiệp vụ: Lấy sản phẩm hiện tại ra và cộng dồn Quantity
                var product = await _productRepo.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    product.Quantity += detail.Quantity;

                    // Cập nhật lại sản phẩm
                    _productRepo.Update(product);
                }
            }

            // 4. Lưu lại toàn bộ các thay đổi (Chi tiết phiếu nhập & Tồn kho sản phẩm) xuống DB
            await _importOrderDetailRepo.SaveChangesAsync();
            await _productRepo.SaveChangesAsync();

            return true;
        }

        public async Task<ImportOrder?> GetImportOrderByIdAsync(int id)
        {
            // Sử dụng GetQueryable() để Join (Include) các bảng lại với nhau
            var query = _importOrderRepo.GetQueryable();

            return await query
                .Include(o => o.Supplier) // Lấy thông tin nhà cung cấp
                .Include(o => o.CreatedByAccount) // Lấy thông tin người tạo phiếu
                .Include(o => o.ImportOrderDetails) // Lấy danh sách các dòng chi tiết
                    .ThenInclude(d => d.Product) // Từ dòng chi tiết, lấy tiếp thông tin Sản phẩm
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}