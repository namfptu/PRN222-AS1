using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;

namespace Razor.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        public IndexModel(IOrderService orderService) => _orderService = orderService;

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync() => Orders = await _orderService.GetAllOrdersAsync();
    }
}
