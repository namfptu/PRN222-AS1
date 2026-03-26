using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public PaginatedList<Product> Products { get; set; } = null!;

        public async Task OnGetAsync(int? pageNumber)
        {
            var all = await _productService.GetAllProductsAsync();
            Products = PaginatedList<Product>.Create(all, pageNumber ?? 1, 10);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                ? "Đã cập nhật trạng thái hoặc xóa sản phẩm thành công!"
                : "Không tìm thấy sản phẩm hoặc có lỗi xảy ra.";
            return RedirectToPage();
        }
    }
}
