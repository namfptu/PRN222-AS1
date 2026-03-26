using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;
using SalesManagement.WebApp.Models;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "ProductManager,Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(
            IProductService productService, 
            ICategoryService categoryService,
            IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var products = await _productService.GetAllProductsAsync();
            int pageSize = 10;
            var paginated = PaginatedList<Product>.Create(products, pageNumber ?? 1, pageSize);
            return View(paginated);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.Status), "Id", "Name");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (await _productService.CodeExistsAsync(product.Code))
            {
                ModelState.AddModelError("Code", "Mã sản phẩm này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    product.ImageUrl = await SaveImage(imageFile);
                }

                await _productService.CreateProductAsync(product);
                TempData["SuccessMessage"] = "Thêm sản phẩm mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.Status), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.Status), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (await _productService.CodeExistsAsync(product.Code, id))
            {
                ModelState.AddModelError("Code", "Mã sản phẩm này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        DeleteOldImage(product.ImageUrl);
                    }
                    product.ImageUrl = await SaveImage(imageFile);
                }

                await _productService.UpdateProductAsync(product);
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.Status), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã cập nhật trạng thái hoặc xóa sản phẩm thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm hoặc có lỗi xảy ra.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/products/" + uniqueFileName;
        }

        private void DeleteOldImage(string imageUrl)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
