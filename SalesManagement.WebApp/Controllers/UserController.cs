using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;
using SalesManagement.WebApp.Models;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;

        public UserController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            var userViewModels = accounts.Select(a => new UserViewModel
            {
                Id = a.Id,
                UserName = a.Email,
                Email = a.Email,
                FullName = a.FullName,
                RoleId = a.Role,
                RoleName = ((AccountRole)a.Role).ToString(),
                IsActive = a.IsActive
            }).ToList();

            return View(userViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _accountService.EmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                var account = new Account
                {
                    Email = model.Email,
                    Password = model.Password, // Hashing should ideally move to Service
                    FullName = model.FullName,
                    Role = model.RoleId,
                    IsActive = true
                };

                await _accountService.CreateAccountAsync(account);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                RoleId = account.Role,
                IsActive = account.IsActive
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _accountService.GetAccountByIdAsync(id);
                    if (account == null)
                    {
                        return NotFound();
                    }

                    // Mapping back to Entity - should ideally use AutoMapper
                    account.Email = model.Email;
                    account.FullName = model.FullName;
                    account.Role = model.RoleId;
                    account.IsActive = model.IsActive;

                    await _accountService.UpdateAccountAsync(account);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _accountService.GetAccountByIdAsync(id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _accountService.DeleteAccountAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ResetPassword(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                Id = account.Id,
                Email = account.Email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, ResetPasswordViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _accountService.ChangePasswordAsync(id, model.NewPassword);
                TempData["SuccessMessage"] = "Password reset successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
