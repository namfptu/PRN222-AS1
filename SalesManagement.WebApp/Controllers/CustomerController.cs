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
            // Sales chi can xem lich su mua hang o muc don hang de tra cuu nhanh.
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

        // Sales mo form them khach hang moi.
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
            // Day la diem them khach hang, kiem tra trung so dien thoai va email truoc khi luu.

            if (await _customerService.PhoneExistsAsync(customer.Phone))
            {
                ModelState.AddModelError("Phone", "So dien thoai nay da ton tai.");
            }

            if (!string.IsNullOrWhiteSpace(customer.Email)
                && await _customerService.EmailExistsAsync(customer.Email))
            {
                ModelState.AddModelError("Email", "Email nay da ton tai.");
            }

            if (ModelState.IsValid)
            {
                await _customerService.CreateCustomerAsync(customer);
                TempData["SuccessMessage"] = "Them khach hang thanh cong!";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // Sales mo form sua thong tin khach hang dang hoat dong.
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
            // Day la diem sua khach hang, van giu rule trung so dien thoai va email nhung bo qua chinh ban ghi hien tai.

            if (await _customerService.PhoneExistsAsync(customer.Phone, customer.Id))
            {
                ModelState.AddModelError("Phone", "So dien thoai nay da ton tai.");
            }

            if (!string.IsNullOrWhiteSpace(customer.Email)
                && await _customerService.EmailExistsAsync(customer.Email, customer.Id))
            {
                ModelState.AddModelError("Email", "Email nay da ton tai.");
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
                TempData["SuccessMessage"] = "Cap nhat khach hang thanh cong!";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // Day la diem xoa mem khach hang trong luong Sales.
        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Da ngung hoat dong khach hang thanh cong!";
            }
            else
            {
                TempData["ErrorMessage"] = "Khong tim thay khach hang!";
            }

            return RedirectToAction(nameof(Index));
        }

        private static void NormalizeCustomer(Customer customer)
        {
            // Gom phan chuan hoa du lieu ve mot cho de luong them va sua xu ly giong nhau.
            customer.FullName = customer.FullName?.Trim() ?? string.Empty;
            customer.Phone = customer.Phone?.Trim() ?? string.Empty;
            customer.Email = string.IsNullOrWhiteSpace(customer.Email) ? null : customer.Email.Trim();
            customer.Address = string.IsNullOrWhiteSpace(customer.Address) ? null : customer.Address.Trim();
        }
    }
}
