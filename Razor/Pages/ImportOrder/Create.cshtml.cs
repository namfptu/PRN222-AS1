using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Razor.Models;
using SalesManagement.Data;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;
using SalesManagement.Data.Entities;

namespace Razor.Pages.ImportOrders
{
    public class CreateModel : PageModel
    {
        private readonly IImportOrderService _importOrderService;
        private readonly ISupplierService _supplierService;
        private readonly SalesManagementDbContext _context;

        public CreateModel(IImportOrderService importOrderService, ISupplierService supplierService, SalesManagementDbContext context)
        {
            _importOrderService = importOrderService;
            _supplierService = supplierService;
            _context = context;
        }

        [BindProperty]
        public CreateImportOrderViewModel Input { get; set; } = new();

        public SelectList SupplierList { get; set; } = null!;
        public Dictionary<int, string> ProductNames { get; set; } = new();
        public Dictionary<int, int> ProductStock { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadSuppliersAsync();
        }

        public async Task<IActionResult> OnPostAsync(string? action)
        {
            await LoadSuppliersAsync(Input.SupplierId);

            if (Input.SupplierId <= 0 || action != "save")
            {
                // Just reload products for the selected supplier
                await LoadProductsForSupplierAsync();
                return Page();
            }

            var details = Input.Details
                .Where(d => d.Quantity > 0 && d.UnitCost > 0)
                .Select(d => new ImportOrderDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost
                }).ToList();

            if (!details.Any())
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập ít nhất 1 sản phẩm với số lượng và giá.");
                await LoadProductsForSupplierAsync();
                return Page();
            }

            var importOrder = new ImportOrder
            {
                SupplierId = Input.SupplierId,
                Note = string.IsNullOrWhiteSpace(Input.Note) ? null : Input.Note.Trim()
            };

            await _importOrderService.CreateImportOrderAsync(importOrder, details);
            TempData["SuccessMessage"] = "Tạo phiếu nhập kho thành công!";
            return RedirectToPage("/ImportOrder/Index");
        }

        private async Task LoadSuppliersAsync(int? selectedId = null)
        {
            var suppliers = await _supplierService.GetActiveSuppliersAsync();
            SupplierList = new SelectList(suppliers, "Id", "CompanyName", selectedId);
        }

        private async Task LoadProductsForSupplierAsync()
        {
            if (Input.SupplierId <= 0) return;

            var products = await _context.Products
                .Where(p => p.ProductSuppliers.Any(ps => ps.SupplierId == Input.SupplierId) && p.Status)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ProductNames = products.ToDictionary(p => p.Id, p => p.Name);
            ProductStock = products.ToDictionary(p => p.Id, p => p.Quantity);

            // Preserve existing quantities/costs if already filled
            var existing = Input.Details.ToDictionary(d => d.ProductId);
            Input.Details = products.Select(p => new ImportOrderDetailViewModel
            {
                ProductId = p.Id,
                Quantity = existing.TryGetValue(p.Id, out var d) ? d.Quantity : 0,
                UnitCost = existing.TryGetValue(p.Id, out var d2) ? d2.UnitCost : 0
            }).ToList();
        }
    }
}
