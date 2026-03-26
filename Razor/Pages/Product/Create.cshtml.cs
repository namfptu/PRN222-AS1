using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Product Input { get; set; } = new();

        public SelectList CategoryList { get; set; } = null!;

        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
        {
            if (await _productService.CodeExistsAsync(Input.Code))
                ModelState.AddModelError("Input.Code", "Mã sản phẩm này đã tồn tại.");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            if (imageFile != null)
                Input.ImageUrl = await SaveImageAsync(imageFile);

            await _productService.CreateProductAsync(Input);
            TempData["SuccessMessage"] = "Thêm sản phẩm mới thành công!";
            return RedirectToPage("/Product/Index");
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            CategoryList = new SelectList(categories.Where(c => c.Status), "Id", "Name");
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
    }
}
