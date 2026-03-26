using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly ICustomerService _customerService;
        public CreateModel(ICustomerService customerService) => _customerService = customerService;

        [BindProperty]
        public Customer Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            Input.FullName = Input.FullName?.Trim() ?? string.Empty;
            Input.Phone = Input.Phone?.Trim() ?? string.Empty;
            Input.Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim();
            Input.Address = string.IsNullOrWhiteSpace(Input.Address) ? null : Input.Address.Trim();

            if (await _customerService.PhoneExistsAsync(Input.Phone))
                ModelState.AddModelError("Input.Phone", "Số điện thoại này đã tồn tại.");

            if (!ModelState.IsValid) return Page();

            await _customerService.CreateCustomerAsync(Input);
            TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
            return RedirectToPage("/Customer/Index");
        }
    }
}
