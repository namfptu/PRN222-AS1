using Microsoft.EntityFrameworkCore;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                // Note: Restrict delete is configured in DbContext, 
                // but we can check here if needed or let the DB throw an exception.
                // For simplicity and following project style, we try to delete.
                try 
                {
                    _categoryRepository.Delete(category);
                    await _categoryRepository.SaveChangesAsync();
                    return true;
                }
                catch
                {
                    // If there are products, it will fail due to Restrict behavior
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var query = _categoryRepository.GetQueryable().Where(c => c.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
