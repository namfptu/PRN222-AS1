using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using System.Security.Claims;

namespace Razor.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly SalesManagementDbContext _context;

        public LoginModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Dashboard/Index");
            }
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return Page();

            var account = _context.Accounts
                .FirstOrDefault(a => a.Email == Input.Email && a.IsActive);

            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại hoặc tài khoản đã bị khóa.");
                return Page();
            }

            if (account.Password != Input.Password)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không chính xác.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.FullName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, ((AccountRole)account.Role).ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(Input.RememberMe ? 30 : 1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToPage("/Dashboard/Index");
        }
    }
}
