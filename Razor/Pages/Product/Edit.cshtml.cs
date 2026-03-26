using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Product Input { get; set; } = new();

        public SelectList CategoryList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            Input = product;
            await LoadCategoriesAsync(product.CategoryId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
        {
            if (await _productService.CodeExistsAsync(Input.Code, Input.Id))
                ModelState.AddModelError("Input.Code", "Mã sản phẩm này đã tồn tại.");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(Input.CategoryId);
                return Page();
            }

            if (imageFile != null)
            {
                if (!string.IsNullOrEmpty(Input.ImageUrl))
                    DeleteOldImage(Input.ImageUrl);
                Input.ImageUrl = await SaveImageAsync(imageFile);
            }

            await _productService.UpdateProductAsync(Input);
            TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
            return RedirectToPage("/Product/Index");
        }

        private async Task LoadCategoriesAsync(int? selected = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            CategoryList = new SelectList(categories.Where(c => c.Status), "Id", "Name", selected);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return "/images/products/" + fileName;
        }

        private void DeleteOldImage(string imageUrl)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        }
    }
}
