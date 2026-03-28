using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;
using SalesManagement.WebApp.Models;
using System.Security.Claims;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "Admin,Sales")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IGenericRepository<Product> _productRepository;

        public OrderController(
            IOrderService orderService,
            ICustomerService customerService,
            IGenericRepository<Product> productRepository)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> Create()
        {
            var model = await BuildCreateOrderViewModelAsync();
            await LoadCustomersAsync(model.CustomerId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Sales")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            // Day la diem Sales tao don ban, chi cac dong co so luong > 0 moi duoc luu vao chi tiet don.
            var selectedItems = model.Items
                .Where(i => i.Quantity > 0)
                .Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                })
                .ToList();

            if (!selectedItems.Any())
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất 1 sản phẩm.");
            }

            foreach (var item in model.Items.Where(i => i.Quantity > i.AvailableQuantity && i.AvailableQuantity > 0))
            {
                ModelState.AddModelError(string.Empty, $"Sản phẩm '{item.ProductName}' chỉ còn {item.AvailableQuantity} trong kho.");
            }

            if (ModelState.IsValid)
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var order = new Order
                {
                    CustomerId = model.CustomerId,
                    Note = string.IsNullOrWhiteSpace(model.Note) ? null : model.Note.Trim(),
                    CreatedBy = currentUserId
                };

                try
                {
                    await _orderService.CreateOrderAsync(order, selectedItems);
                    TempData["SuccessMessage"] = "Tạo đơn hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var rebuiltModel = await BuildCreateOrderViewModelAsync(model);
            await LoadCustomersAsync(model.CustomerId);
            return View(rebuiltModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            // Sales xem chi tiet don tai day va tu man nay co the in hoa don.
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Sales")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status, string? returnTo = null)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, status);
                TempData["SuccessMessage"] = status == OrderStatus.Done
                    ? "Cập nhật đơn hàng sang Hoàn thành thành công!"
                    : "Đã hủy đơn hàng thành công!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            if (string.Equals(returnTo, "Details", StringComparison.OrdinalIgnoreCase))
            {
                // Dung chung 1 action cap nhat trang thai cho ca man danh sach va man chi tiet.
                return RedirectToAction(nameof(Details), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<CreateOrderViewModel> BuildCreateOrderViewModelAsync(CreateOrderViewModel? sourceModel = null)
        {
            var quantityLookup = sourceModel?.Items?
                .ToDictionary(i => i.ProductId, i => i.Quantity) ?? new Dictionary<int, int>();

            // Doc ton kho moi nhat tu DB de form tao don luon dung voi so luong co the ban.
            var products = await _productRepository.GetQueryable()
                .Where(p => p.Status && p.Quantity > 0)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return new CreateOrderViewModel
            {
                CustomerId = sourceModel?.CustomerId,
                Note = sourceModel?.Note,
                Items = products.Select(p => new CreateOrderItemViewModel
                {
                    ProductId = p.Id,
                    ProductCode = p.Code,
                    ProductName = p.Name,
                    UnitPrice = p.Price,
                    AvailableQuantity = p.Quantity,
                    Quantity = quantityLookup.TryGetValue(p.Id, out var quantity) ? quantity : 0
                }).ToList()
            };
        }

        private async Task LoadCustomersAsync(int? selectedCustomerId = null)
        {
            // Chi cho phep chon khach hang dang hoat dong khi Sales tao don moi.
            var customers = await _customerService.GetActiveCustomersAsync();
            ViewBag.Customers = new SelectList(
                customers.Select(c => new
                {
                    c.Id,
                    DisplayName = $"{c.FullName} - {c.Phone}"
                }),
                "Id",
                "DisplayName",
                selectedCustomerId);
        }
    }
}
