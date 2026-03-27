using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly SalesManagementDbContext _context;

        public ProductService(IProductRepository productRepository, SalesManagementDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // We want to include Category for the list view
            return await _productRepository.GetQueryable()
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync()
        {
            return await _productRepository.GetAllWithDetailsAsync();
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

            // Chặn xóa nếu đã có đơn bán hoặc đơn nhập
            if (product.OrderDetails.Any() || product.ImportOrderDetails.Any())
            {
                return false; // Trạng thái không đổi
            }

            _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            product.Status = !product.Status;
            _productRepository.Update(product);
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

        public async Task AddProductSupplierAsync(int productId, int supplierId)
        {
            var exists = await _context.Set<ProductSupplier>()
                .AnyAsync(ps => ps.ProductId == productId && ps.SupplierId == supplierId);
            if (!exists)
            {
                _context.Set<ProductSupplier>().Add(new ProductSupplier
                {
                    ProductId = productId,
                    SupplierId = supplierId
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateProductSuppliersAsync(int productId, int supplierId)
        {
            var existingItems = _context.Set<ProductSupplier>().Where(ps => ps.ProductId == productId);
            _context.Set<ProductSupplier>().RemoveRange(existingItems);
            
            _context.Set<ProductSupplier>().Add(new ProductSupplier
            {
                ProductId = productId,
                SupplierId = supplierId
            });
            await _context.SaveChangesAsync();
        }
    }
}
