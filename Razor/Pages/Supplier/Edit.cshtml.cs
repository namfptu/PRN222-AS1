using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Suppliers
{
    public class EditModel : PageModel
    {
        private readonly ISupplierService _supplierService;
        public EditModel(ISupplierService supplierService) => _supplierService = supplierService;

        [BindProperty]
        public Supplier Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null) return NotFound();
            Input = supplier;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _supplierService.UpdateSupplierAsync(Input);
            TempData["SuccessMessage"] = "Cập nhật thông tin nhà cung cấp thành công!";
            return RedirectToPage("/Supplier/Index");
        }
    }
}
