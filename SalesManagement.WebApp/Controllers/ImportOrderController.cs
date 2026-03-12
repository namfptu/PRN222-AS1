using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;
using SalesManagement.WebApp.Models;
using System.Security.Claims;

namespace SalesManagement.WebApp.Controllers
{
    public class ImportOrderController : Controller
    {
        private readonly IImportOrderService _importOrderService;
        private readonly ISupplierService _supplierService;
        private readonly IGenericRepository<Product> _productRepo; // Tạm dùng Repo vì TV2 chưa làm xong ProductService

        public ImportOrderController(
            IImportOrderService importOrderService,
            ISupplierService supplierService,
            IGenericRepository<Product> productRepo)
        {
            _importOrderService = importOrderService;
            _supplierService = supplierService;
            _productRepo = productRepo;
        }

        // GET: Hiển thị danh sách phiếu nhập
        public async Task<IActionResult> Index()
        {
            var orders = await _importOrderService.GetAllImportOrdersAsync();
            return View(orders);
        }

        // GET: Form tạo phiếu nhập mới
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách Nhà cung cấp (chỉ lấy Active) để nạp vào Dropdown
            var suppliers = await _supplierService.GetActiveSuppliersAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "Id", "CompanyName");

            // Lấy danh sách Sản phẩm nạp vào Dropdown
            var products = await _productRepo.GetAllAsync();
            ViewBag.Products = new SelectList(products, "Id", "Name");

            return View(new CreateImportOrderViewModel());
        }

        // POST: Xử lý lưu phiếu nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: API lấy danh sách Sản phẩm theo Nhà cung cấp (Dùng cho AJAX)
        [HttpGet]
        public IActionResult GetProductsBySupplier(int supplierId)
        {
            // Dùng GetQueryable() từ GenericRepository để truy vấn LINQ
            var query = _productRepo.GetQueryable();

            // Tìm các sản phẩm có liên kết với SupplierId này thông qua bảng trung gian ProductSuppliers
            var products = query.Where(p => p.ProductSuppliers.Any(ps => ps.SupplierId == supplierId))
                                .Select(p => new
                                {
                                    id = p.Id,
                                    name = p.Name
                                })
                                .ToList();

            // Trả về dữ liệu dạng JSON cho JavaScript đọc
            return Json(products);
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