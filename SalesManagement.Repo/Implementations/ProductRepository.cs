using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;

namespace SalesManagement.Repo.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(SalesManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        public async Task<Product?> GetWithCategoryByIdAsync(int id)
        {
            return await _dbSet.Include(p => p.Category)
                               .Include(p => p.ProductSuppliers)
                               .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _dbSet.Include(p => p.Category)
                               .Include(p => p.ProductSuppliers)
                               .Include(p => p.ImportOrderDetails)
                                    .ThenInclude(iod => iod.ImportOrder)
                               .ToListAsync();
        }
    }
}
