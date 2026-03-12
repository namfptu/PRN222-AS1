using SalesManagement.Data.Entities;

namespace SalesManagement.Service.Interfaces
{
    public interface IReportService
    {
        // Hàm lấy sản phẩm sắp hết hàng với mức cảnh báo mặc định là 10
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
    }
}