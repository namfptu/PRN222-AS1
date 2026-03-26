using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Data;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public EditModel(SalesManagementDbContext context) => _context = context;

        [BindProperty]
        public EditUserViewModel Input { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) return NotFound();
            Input = new EditUserViewModel
            {
                Id = account.Id, FullName = account.FullName,
                Email = account.Email, RoleId = account.Role, IsActive = account.IsActive
            };
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var account = _context.Accounts.FirstOrDefault(a => a.Id == Input.Id);
            if (account == null) return NotFound();

            account.FullName = Input.FullName.Trim();
            account.Role = Input.RoleId;
            account.IsActive = Input.IsActive;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
            return RedirectToPage("/User/Index");
        }
    }
}
