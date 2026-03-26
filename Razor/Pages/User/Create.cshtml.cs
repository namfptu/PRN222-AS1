using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Data;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public CreateModel(SalesManagementDbContext context) => _context = context;

        [BindProperty]
        public CreateUserViewModel Input { get; set; } = new();

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (_context.Accounts.Any(a => a.Email == Input.Email))
                ModelState.AddModelError("Input.Email", "Email này đã được sử dụng.");

            if (!ModelState.IsValid) return Page();

            var account = new Account
            {
                FullName = Input.FullName.Trim(),
                Email = Input.Email.Trim(),
                Password = Input.Password,
                Role = Input.RoleId,
                IsActive = true
            };

            _context.Accounts.Add(account);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Thêm người dùng thành công!";
            return RedirectToPage("/User/Index");
        }
    }
}
