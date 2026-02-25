using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.WebApp.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: Hiển thị danh sách nhà cung cấp
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return View(suppliers);
        }

        // GET: Hiển thị form thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Xử lý lưu nhà cung cấp mới vào DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.CreateSupplierAsync(supplier);
                TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }
    }
}