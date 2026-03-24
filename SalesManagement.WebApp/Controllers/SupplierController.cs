using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "Warehouse,Admin")]
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
            var activeSuppliers = await _supplierService.GetActiveSuppliersAsync();
            return View(activeSuppliers);
        }

        // GET: Hiển thị form thêm mới
        [Authorize(Roles = "Warehouse")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Xử lý lưu nhà cung cấp mới vào DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Warehouse")]
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

        // GET: Hiển thị form cập nhật thông tin
        [Authorize(Roles = "Warehouse")]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Xử lý lưu thông tin sau khi cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Warehouse")]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _supplierService.UpdateSupplierAsync(supplier);
                TempData["SuccessMessage"] = "Cập nhật thông tin nhà cung cấp thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Xử lý ngừng hoạt động (Xóa mềm)
        // Lưu ý: Ở trang Index, nút Xóa đang gọi trực tiếp đến hàm này
        [Authorize(Roles = "Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã chuyển trạng thái nhà cung cấp thành Ngừng hoạt động!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}