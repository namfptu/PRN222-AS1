using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;

namespace Razor.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        public IndexModel(ICustomerService customerService) => _customerService = customerService;

        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();

        public async Task OnGetAsync() => Customers = await _customerService.GetActiveCustomersAsync();

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result ? "Đã ngừng hoạt động khách hàng!" : "Không tìm thấy khách hàng!";
            return RedirectToPage();
        }
    }
}
