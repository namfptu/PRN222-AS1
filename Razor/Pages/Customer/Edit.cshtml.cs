using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Service.Interfaces;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly ICustomerService _customerService;
        public EditModel(ICustomerService customerService) => _customerService = customerService;

        [BindProperty]
        public Customer Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null || !customer.Status) return NotFound();
            Input = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Input.FullName = Input.FullName?.Trim() ?? string.Empty;
            Input.Phone = Input.Phone?.Trim() ?? string.Empty;
            Input.Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim();
            Input.Address = string.IsNullOrWhiteSpace(Input.Address) ? null : Input.Address.Trim();

            if (await _customerService.PhoneExistsAsync(Input.Phone, Input.Id))
                ModelState.AddModelError("Input.Phone", "Số điện thoại này đã tồn tại.");

            if (!ModelState.IsValid) return Page();

            var existing = await _customerService.GetCustomerByIdAsync(Input.Id);
            if (existing == null) return NotFound();
            existing.FullName = Input.FullName;
            existing.Phone = Input.Phone;
            existing.Email = Input.Email;
            existing.Address = Input.Address;

            await _customerService.UpdateCustomerAsync(existing);
            TempData["SuccessMessage"] = "Cập nhật khách hàng thành công!";
            return RedirectToPage("/Customer/Index");
        }
    }
}
