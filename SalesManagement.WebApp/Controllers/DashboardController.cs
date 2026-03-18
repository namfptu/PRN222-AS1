using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using System.Security.Claims;

namespace SalesManagement.WebApp.Controllers
{
    [Authorize] // Yêu cầu đăng nhập
    public class DashboardController : Controller
    {
        private readonly SalesManagementDbContext _context;

        public DashboardController(SalesManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var isAdmin = User.IsInRole("Admin");
            var isSales = User.IsInRole("Sales") || isAdmin;
            var isProduct = User.IsInRole("ProductManager") || isAdmin;
            var isWarehouse = User.IsInRole("Warehouse") || isAdmin;

            int userId = 0;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);

            // ==========================================
            // 1. DỮ LIỆU BÁN HÀNG (SALES)
            // ==========================================
            if (isSales)
            {
                if (User.IsInRole("Sales")) // Chỉ lấy đơn của chính mình nếu là Sales
                {
                    ViewBag.TotalOrders = _context.Orders.Count(o => o.CreatedBy == userId);
                    ViewBag.TodayOrders = _context.Orders.Count(o => o.CreatedBy == userId && o.CreatedDate.Date == DateTime.Today);
                    
                    ViewBag.TotalRevenue = _context.Orders
                        .Where(o => o.CreatedBy == userId && o.Status == Data.Entities.OrderStatus.Done)
                        .Sum(o => o.TotalAmount);
                    
                    ViewBag.TodayRevenue = _context.Orders
                        .Where(o => o.CreatedBy == userId && o.Status == Data.Entities.OrderStatus.Done && o.CreatedDate.Date == DateTime.Today)
                        .Sum(o => o.TotalAmount);

                    ViewBag.RecentOrders = _context.Orders
                        .Where(o => o.CreatedBy == userId)
                        .OrderByDescending(o => o.CreatedDate)
                        .Take(5)
                        .ToList();
                }
                else // Admin: lấy toàn bộ
                {
                    ViewBag.TotalOrders = _context.Orders.Count();
                    ViewBag.TodayOrders = _context.Orders.Count(o => o.CreatedDate.Date == DateTime.Today);
                    
                    ViewBag.TotalRevenue = _context.Orders
                        .Where(o => o.Status == Data.Entities.OrderStatus.Done)
                        .Sum(o => o.TotalAmount);
                    
                    ViewBag.TodayRevenue = _context.Orders
                        .Where(o => o.Status == Data.Entities.OrderStatus.Done && o.CreatedDate.Date == DateTime.Today)
                        .Sum(o => o.TotalAmount);

                    ViewBag.RecentOrders = _context.Orders
                        .OrderByDescending(o => o.CreatedDate)
                        .Take(5)
                        .ToList();
                }
                ViewBag.TotalCustomers = _context.Customers.Count();
            }

            // ==========================================
            // 2. DỮ LIỆU SẢN PHẨM (PRODUCT)
            // ==========================================
            if (isProduct)
            {
                ViewBag.TotalProducts = _context.Products.Count();
                ViewBag.TotalCategories = _context.Categories.Count();
                
                ViewBag.LowStockProducts = _context.Products
                    .Where(p => p.Quantity < 10 && p.Status)
                    .OrderBy(p => p.Quantity)
                    .Take(5)
                    .ToList();
            }

            // ==========================================
            // 3. DỮ LIỆU KHO HÀNG (WAREHOUSE)
            // ==========================================
            if (isWarehouse)
            {
                ViewBag.TotalSuppliers = _context.Suppliers.Count();
                ViewBag.TodayImports = _context.ImportOrders.Count(io => io.ImportDate.Date == DateTime.Today);
                ViewBag.TotalImportsAmount = _context.ImportOrders.Sum(io => io.TotalCost);

                ViewBag.RecentImports = _context.ImportOrders
                    .Include(io => io.Supplier)
                    .OrderByDescending(io => io.ImportDate)
                    .Take(5)
                    .ToList();
            }

            return View();
        }
    }
}
