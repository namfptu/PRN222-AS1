using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Models;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using System.Security.Claims;

namespace Razor.Pages.Auth
{
    public class ProfileModel : PageModel
    {
        private readonly SalesManagementDbContext _context;

        public ProfileModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProfileViewModel Profile { get; set; } = new();

        [BindProperty]
        public ChangePasswordViewModel PasswordInput { get; set; } = new();

        public IActionResult OnGet()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var account = _context.Accounts.Include(a => a.Profile).FirstOrDefault(a => a.Id == userId);
            if (account == null) return NotFound();

            Profile = new ProfileViewModel
            {
                Email = account.Email,
                FullName = account.FullName,
                Role = ((AccountRole)account.Role).ToString(),
                PhoneNumber = account.Profile?.PhoneNumber,
                Address = account.Profile?.Address,
                DateOfBirth = account.Profile?.DateOfBirth,
                Avatar = account.Profile?.Avatar,
                JoinDate = account.Profile?.JoinDate ?? DateTime.Now
            };
            return Page();
        }

        public IActionResult OnPostUpdateProfile()
        {
            // Only validate Profile, clear PasswordInput errors
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("PasswordInput")).ToList())
                ModelState.Remove(key);

            if (!ModelState.IsValid)
                return Page();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var account = _context.Accounts.Include(a => a.Profile).FirstOrDefault(a => a.Id == userId);
            if (account == null) return NotFound();

            account.FullName = Profile.FullName;

            if (account.Profile == null)
            {
                account.Profile = new AccountProfile { AccountId = userId, JoinDate = DateTime.Now };
            }

            account.Profile.PhoneNumber = Profile.PhoneNumber;
            account.Profile.Address = Profile.Address;
            account.Profile.DateOfBirth = Profile.DateOfBirth;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToPage();
        }

        public IActionResult OnPostChangePassword()
        {
            // Only validate PasswordInput, clear Profile errors
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Profile")).ToList())
                ModelState.Remove(key);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào.";
                return RedirectToPage();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var account = _context.Accounts.FirstOrDefault(a => a.Id == userId);
            if (account == null) return NotFound();

            if (account.Password != PasswordInput.CurrentPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không chính xác.";
                return RedirectToPage();
            }

            account.Password = PasswordInput.NewPassword;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToPage();
        }
    }
}
