using SalesManagement.Data.Entities;

namespace SalesManagement.Repo.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
        Task<Product?> GetWithCategoryByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();
    }
}
