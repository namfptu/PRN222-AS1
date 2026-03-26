using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Suppliers
{
    public class CreateModel : PageModel
    {
        private readonly ISupplierService _supplierService;
        public CreateModel(ISupplierService supplierService) => _supplierService = supplierService;

        [BindProperty]
        public Supplier Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _supplierService.CreateSupplierAsync(Input);
            TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
            return RedirectToPage("/Supplier/Index");
        }
    }
}
