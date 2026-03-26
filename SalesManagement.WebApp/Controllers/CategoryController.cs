using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;
using SalesManagement.WebApp.Models;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "ProductManager,Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Category
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            int pageSize = 10;
            var paginated = PaginatedList<Category>.Create(categories, pageNumber ?? 1, pageSize);
            return View(paginated);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (await _categoryService.NameExistsAsync(category.Name))
            {
                ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(category);
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (await _categoryService.NameExistsAsync(category.Name, id))
            {
                ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục này (có thể do danh mục còn chứa sản phẩm).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
