using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<Product> _productRepo;

        public ReportService(IGenericRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            // Truy vấn trực tiếp xuống DB: Chỉ lấy những sản phẩm có Quantity <= threshold
            return await _productRepo.GetAsync(p => p.Quantity <= threshold);
        }
    }
}