using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using SalesManagement.WebApp.Models;
using System.Security.Claims;

namespace SalesManagement.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly SalesManagementDbContext _context;

        public AuthController(SalesManagementDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu đã đăng nhập, chuyển đến Dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm user theo email
            var account = _context.Accounts
                .FirstOrDefault(a => a.Email == model.Email && a.IsActive);

            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại hoặc tài khoản đã bị khóa.");
                return View(model);
            }

            // Kiểm tra mật khẩu (TODO: Sử dụng hash trong production)
            if (account.Password != model.Password)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không chính xác.");
                return View(model);
            }

            // Tạo Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.FullName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role == 1 ? "Admin" : "Staff")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(model.RememberMe ? 30 : 1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect về returnUrl hoặc Dashboard
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Auth/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: /Auth/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Auth/Profile
        [HttpGet]
        public IActionResult Profile()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Include bảng Profile để lấy thông tin chi tiết
            var account = _context.Accounts
                .Include(a => a.Profile)
                .FirstOrDefault(a => a.Id == userId);

            if (account == null)
            {
                return NotFound();
            }

            // Map Entity -> ViewModel
            var model = new ProfileViewModel
            {
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role == 1 ? "Admin" : "Staff",
                PhoneNumber = account.Profile?.PhoneNumber,
                Address = account.Profile?.Address,
                DateOfBirth = account.Profile?.DateOfBirth,
                Avatar = account.Profile?.Avatar,
                JoinDate = account.Profile?.JoinDate ?? DateTime.Now
            };

            return View(model);
        }

        // POST: /Auth/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var account = _context.Accounts
                .Include(a => a.Profile)
                .FirstOrDefault(a => a.Id == userId);

            if (account == null)
            {
                return NotFound();
            }

            // Update Account info
            account.FullName = model.FullName;

            // Update or Create Profile info
            if (account.Profile == null)
            {
                account.Profile = new AccountProfile
                {
                    AccountId = userId,
                    JoinDate = DateTime.Now
                };
            }

            account.Profile.PhoneNumber = model.PhoneNumber;
            account.Profile.Address = model.Address;
            account.Profile.DateOfBirth = model.DateOfBirth;
            // Avatar update logic later

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }
    }
}
