using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;
using SalesManagement.WebApp.Models;
using System.Security.Claims;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "Warehouse,Admin")]
    public class ImportOrderController : Controller
    {
        private readonly IImportOrderService _importOrderService;
        private readonly ISupplierService _supplierService;
        private readonly IGenericRepository<Product> _productRepo; // Tạm dùng Repo vì TV2 chưa làm xong ProductService
        private readonly IGenericRepository<Supplier> _supplierRepo;

        public ImportOrderController(
            IImportOrderService importOrderService,
            ISupplierService supplierService,
            IGenericRepository<Product> productRepo,
            IGenericRepository<Supplier> supplierRepo)
        {
            _importOrderService = importOrderService;
            _supplierService = supplierService;
            _productRepo = productRepo;
            _supplierRepo = supplierRepo;
        }

        // GET: Hiển thị danh sách phiếu nhập
        public async Task<IActionResult> Index()
        {
            var orders = await _importOrderService.GetAllImportOrdersAsync();
            return View(orders);
        }

        // GET: Hiển thị form tạo phiếu nhập
        [Authorize(Roles = "Warehouse")]
        public IActionResult Create(int? selectedSupplierId) // Thêm tham số nhận ID từ URL
        {
            // 1. Load danh sách Nhà cung cấp (Giữ nguyên)
            var suppliers = _supplierRepo.GetQueryable().Where(s => s.Status == true).ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "Id", "CompanyName", selectedSupplierId);

            // 2. Thuần MVC: Nếu URL có truyền lên selectedSupplierId, ta load sẵn Sản phẩm gửi ra View
            if (selectedSupplierId.HasValue)
            {
                var products = _productRepo.GetQueryable()
                    .Where(p => p.ProductSuppliers.Any(ps => ps.SupplierId == selectedSupplierId.Value))
                    .Select(p => new { Id = p.Id, Name = p.Name })
                    .ToList();

                ViewBag.Products = new SelectList(products, "Id", "Name");
                ViewBag.SelectedSupplierId = selectedSupplierId.Value; // Lưu lại để View biết
            }
            else
            {
                ViewBag.Products = null; // Chưa chọn nhà cung cấp thì rỗng
            }

            return View();
        }

        // POST: Xử lý lưu phiếu nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Warehouse")]
        public async Task<IActionResult> Create(CreateImportOrderViewModel model)
        {
            // Lấy UserId của người tạo phiếu nhập (để lưu vào CreatedBy)
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (ModelState.IsValid && model.Details != null && model.Details.Any())
            {
                var order = new ImportOrder
                {
                    SupplierId = model.SupplierId,
                    Note = model.Note,
                    CreatedBy = currentUserId
                };

                var details = model.Details.Select(d => new ImportOrderDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost
                }).ToList();

                // Gọi Service thực hiện Transaction (Lưu phiếu + Cộng tồn kho)
                await _importOrderService.CreateImportOrderAsync(order, details);

                TempData["SuccessMessage"] = "Nhập kho thành công! Số lượng sản phẩm đã được cộng dồn.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi (ví dụ submit form rỗng), load lại dữ liệu cho Dropdown và báo lỗi
            var allSuppliers = await _supplierService.GetAllSuppliersAsync();
            ViewBag.Suppliers = new SelectList(allSuppliers.Where(s => s.Status == true), "Id", "CompanyName");

            ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 sản phẩm để nhập kho.");
            return View(model);
        }

        // GET: Xem chi tiết phiếu nhập
        public async Task<IActionResult> Details(int id)
        {
            var order = await _importOrderService.GetImportOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}