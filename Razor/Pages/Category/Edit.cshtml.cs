using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public EditModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public Category Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            Input = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _categoryService.NameExistsAsync(Input.Name, Input.Id))
                ModelState.AddModelError("Input.Name", "Tên danh mục này đã tồn tại.");

            if (!ModelState.IsValid)
                return Page();

            await _categoryService.UpdateCategoryAsync(Input);
            TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            return RedirectToPage("/Category/Index");
        }
    }
}
