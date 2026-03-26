using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Customers
{
    public class HistoryModel : PageModel
    {
        private readonly ICustomerService _customerService;
        public HistoryModel(ICustomerService customerService) => _customerService = customerService;

        public CustomerPurchaseHistoryViewModel Data { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            var orders = (await _customerService.GetCustomerPurchaseHistoryAsync(id)).ToList();
            Data = new CustomerPurchaseHistoryViewModel
            {
                Customer = customer,
                TotalOrders = orders.Count,
                TotalSpent = orders.Sum(o => o.TotalAmount),
                Orders = orders.Select(o => new CustomerOrderHistoryItemViewModel
                {
                    Id = o.Id, Code = o.Code, CreatedDate = o.CreatedDate,
                    TotalAmount = o.TotalAmount, Status = o.Status, Note = o.Note
                }).ToList()
            };
            return Page();
        }
    }
}
