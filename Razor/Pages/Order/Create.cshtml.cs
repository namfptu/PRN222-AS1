using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Razor.Models;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;
using System.Security.Claims;

namespace Razor.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IGenericRepository<Product> _productRepository;

        public CreateModel(IOrderService orderService, ICustomerService customerService, IGenericRepository<Product> productRepository)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productRepository = productRepository;
        }

        [BindProperty]
        public CreateOrderViewModel Input { get; set; } = new();

        public SelectList CustomerList { get; set; } = null!;

        public async Task OnGetAsync()
        {
            Input = await BuildViewModelAsync();
            await LoadCustomersAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var selectedItems = Input.Items.Where(i => i.Quantity > 0)
                .Select(i => new OrderDetail { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();

            if (!selectedItems.Any())
                ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất 1 sản phẩm.");

            foreach (var item in Input.Items.Where(i => i.Quantity > i.AvailableQuantity && i.AvailableQuantity > 0))
                ModelState.AddModelError(string.Empty, $"Sản phẩm '{item.ProductName}' chỉ còn {item.AvailableQuantity} trong kho.");

            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var order = new Order
                {
                    CustomerId = Input.CustomerId,
                    Note = string.IsNullOrWhiteSpace(Input.Note) ? null : Input.Note.Trim(),
                    CreatedBy = userId
                };

                try
                {
                    await _orderService.CreateOrderAsync(order, selectedItems);
                    TempData["SuccessMessage"] = "Tạo đơn hàng thành công!";
                    return RedirectToPage("/Order/Index");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            Input = await BuildViewModelAsync(Input);
            await LoadCustomersAsync(Input.CustomerId);
            return Page();
        }

        private async Task<CreateOrderViewModel> BuildViewModelAsync(CreateOrderViewModel? source = null)
        {
            var qtyLookup = source?.Items?.ToDictionary(i => i.ProductId, i => i.Quantity) ?? new();
            var products = await _productRepository.GetQueryable()
                .Where(p => p.Status && p.Quantity > 0).OrderBy(p => p.Name).ToListAsync();
            return new CreateOrderViewModel
            {
                CustomerId = source?.CustomerId,
                Note = source?.Note,
                Items = products.Select(p => new CreateOrderItemViewModel
                {
                    ProductId = p.Id, ProductCode = p.Code, ProductName = p.Name,
                    UnitPrice = p.Price, AvailableQuantity = p.Quantity,
                    Quantity = qtyLookup.TryGetValue(p.Id, out var q) ? q : 0
                }).ToList()
            };
        }

        private async Task LoadCustomersAsync(int? selectedId = null)
        {
            var customers = await _customerService.GetActiveCustomersAsync();
            CustomerList = new SelectList(customers.Select(c => new { c.Id, DisplayName = $"{c.FullName} - {c.Phone}" }), "Id", "DisplayName", selectedId);
        }
    }
}
