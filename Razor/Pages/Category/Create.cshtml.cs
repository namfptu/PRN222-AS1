using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CreateModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public Category Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _categoryService.NameExistsAsync(Input.Name))
                ModelState.AddModelError("Input.Name", "Tên danh mục này đã tồn tại.");

            if (!ModelState.IsValid)
                return Page();

            await _categoryService.CreateCategoryAsync(Input);
            TempData["SuccessMessage"] = "Thêm danh mục thành công!";
            return RedirectToPage("/Category/Index");
        }
    }
}
