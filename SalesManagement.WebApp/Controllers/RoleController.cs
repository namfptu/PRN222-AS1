using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Role
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: Role/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Tên role không được để trống");
                return View();
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError("", "Role này đã tồn tại");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Tạo role '{roleName}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        // POST: Role/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy role";
                return RedirectToAction(nameof(Index));
            }

            // Prevent deleting system roles
            if (role.Name == "Admin" || role.Name == "User" || role.Name == "Moderator")
            {
                TempData["ErrorMessage"] = "Không thể xóa role hệ thống";
                return RedirectToAction(nameof(Index));
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Xóa role '{role.Name}' thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa role thất bại";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
