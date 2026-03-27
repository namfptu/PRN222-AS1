using SalesManagement.Data.Entities;

namespace SalesManagement.Service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task AddProductSupplierAsync(int productId, int supplierId);
        Task UpdateProductSuppliersAsync(int productId, int supplierId);
        Task<bool> ToggleStatusAsync(int id);
    }
}
