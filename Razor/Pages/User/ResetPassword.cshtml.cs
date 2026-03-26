using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Data;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Users
{
    public class ResetPasswordModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public ResetPasswordModel(SalesManagementDbContext context) => _context = context;

        [BindProperty]
        public ResetPasswordViewModel Input { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) return NotFound();
            Input = new ResetPasswordViewModel { Id = account.Id, Email = account.Email };
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var account = _context.Accounts.FirstOrDefault(a => a.Id == Input.Id);
            if (account == null) return NotFound();

            account.Password = Input.NewPassword;
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Đặt lại mật khẩu cho {Input.Email} thành công!";
            return RedirectToPage("/User/Index");
        }
    }
}
