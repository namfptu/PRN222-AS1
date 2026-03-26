using Microsoft.EntityFrameworkCore;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // We want to include Category for the list view
            return await _productRepository.GetQueryable()
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetWithCategoryByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetQueryable()
                .Include(p => p.OrderDetails)
                .Include(p => p.ImportOrderDetails)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return false;

            // Logic check: "Không xóa sản phẩm đã có đơn hàng -> Chuyển trạng thái ngưng bán"
            if (product.OrderDetails.Any() || product.ImportOrderDetails.Any())
            {
                product.Status = false; // Discontinued
                _productRepository.Update(product);
            }
            else
            {
                _productRepository.Delete(product);
            }

            await _productRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _productRepository.GetByCategoryIdAsync(categoryId);
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _productRepository.GetQueryable().Where(p => p.Code == code);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
