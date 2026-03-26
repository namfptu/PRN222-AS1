using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Service.Interfaces;

namespace Razor.Pages.Suppliers
{
    public class IndexModel : PageModel
    {
        private readonly ISupplierService _supplierService;
        public IndexModel(ISupplierService supplierService) => _supplierService = supplierService;

        public IEnumerable<SalesManagement.Data.Entities.Supplier> Suppliers { get; set; } = new List<SalesManagement.Data.Entities.Supplier>();

        public async Task OnGetAsync() => Suppliers = await _supplierService.GetActiveSuppliersAsync();

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                ? "Đã chuyển trạng thái nhà cung cấp thành Ngừng hoạt động!"
                : "Không tìm thấy nhà cung cấp!";
            return RedirectToPage();
        }
    }
}
