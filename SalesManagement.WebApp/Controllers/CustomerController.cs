using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;
using SalesManagement.WebApp.Models;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize(Roles = "Admin,Sales")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetActiveCustomersAsync();
            return View(customers);
        }

        public async Task<IActionResult> History(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var orders = (await _customerService.GetCustomerPurchaseHistoryAsync(id)).ToList();
            var model = new CustomerPurchaseHistoryViewModel
            {
                Customer = customer,
                TotalOrders = orders.Count,
                TotalSpent = orders.Sum(o => o.TotalAmount),
                Orders = orders.Select(o => new CustomerOrderHistoryItemViewModel
                {
                    Id = o.Id,
                    Code = o.Code,
                    CreatedDate = o.CreatedDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    Note = o.Note
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Sales")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Sales")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            NormalizeCustomer(customer);

            if (await _customerService.PhoneExistsAsync(customer.Phone))
            {
                ModelState.AddModelError("Phone", "Số điện thoại này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                await _customerService.CreateCustomerAsync(customer);
                TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null || !customer.Status)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [Authorize(Roles = "Sales")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            NormalizeCustomer(customer);

            if (await _customerService.PhoneExistsAsync(customer.Phone, customer.Id))
            {
                ModelState.AddModelError("Phone", "Số điện thoại này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                var existingCustomer = await _customerService.GetCustomerByIdAsync(id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                existingCustomer.FullName = customer.FullName;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Email = customer.Email;
                existingCustomer.Address = customer.Address;

                await _customerService.UpdateCustomerAsync(existingCustomer);
                TempData["SuccessMessage"] = "Cập nhật khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã ngừng hoạt động khách hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
            }

            return RedirectToAction(nameof(Index));
        }

        private static void NormalizeCustomer(Customer customer)
        {
            customer.FullName = customer.FullName?.Trim() ?? string.Empty;
            customer.Phone = customer.Phone?.Trim() ?? string.Empty;
            customer.Email = string.IsNullOrWhiteSpace(customer.Email) ? null : customer.Email.Trim();
            customer.Address = string.IsNullOrWhiteSpace(customer.Address) ? null : customer.Address.Trim();
        }
    }
}
