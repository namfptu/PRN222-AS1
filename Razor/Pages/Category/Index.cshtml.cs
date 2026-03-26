using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;

namespace Razor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public PaginatedList<Category> Categories { get; set; } = null!;

        public async Task OnGetAsync(int? pageNumber)
        {
            var all = await _categoryService.GetAllCategoriesAsync();
            Categories = PaginatedList<Category>.Create(all, pageNumber ?? 1, 10);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                ? "Xóa danh mục thành công!"
                : "Không thể xóa danh mục này (có thể do danh mục còn chứa sản phẩm).";
            return RedirectToPage();
        }
    }
}
