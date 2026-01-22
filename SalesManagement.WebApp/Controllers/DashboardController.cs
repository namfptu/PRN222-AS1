using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var isStaff = User.IsInRole("Staff");
            int userId = 0;
            
            if (isStaff)
            {
                // Lấy UserId của Staff đang đăng nhập
                int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
            }

            // 1. Số lượng đơn hàng & Doanh thu
            if (isStaff)
            {
                // Staff: Chỉ tính đơn của mình
                ViewBag.TotalOrders = _context.Orders.Count(o => o.CreatedBy == userId);
                ViewBag.TodayOrders = _context.Orders.Count(o => o.CreatedBy == userId && o.CreatedDate.Date == DateTime.Today);
                
                ViewBag.TotalRevenue = _context.Orders
                    .Where(o => o.CreatedBy == userId && o.Status == Data.Entities.OrderStatus.Done)
                    .Sum(o => o.TotalAmount);
                
                ViewBag.TodayRevenue = _context.Orders
                    .Where(o => o.CreatedBy == userId && o.Status == Data.Entities.OrderStatus.Done && o.CreatedDate.Date == DateTime.Today)
                    .Sum(o => o.TotalAmount);
            }
            else
            {
                // Admin: Tính toàn bộ
                ViewBag.TotalOrders = _context.Orders.Count();
                ViewBag.TodayOrders = _context.Orders.Count(o => o.CreatedDate.Date == DateTime.Today);
                
                ViewBag.TotalRevenue = _context.Orders
                    .Where(o => o.Status == Data.Entities.OrderStatus.Done)
                    .Sum(o => o.TotalAmount);
                
                ViewBag.TodayRevenue = _context.Orders
                    .Where(o => o.Status == Data.Entities.OrderStatus.Done && o.CreatedDate.Date == DateTime.Today)
                    .Sum(o => o.TotalAmount);
            }

            // 2. Thông tin chung (Cả 2 đều thấy giống nhau)
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalCustomers = _context.Customers.Count();

            // 3. Đơn hàng gần đây
            if (isStaff)
            {
                // Staff: Chỉ thấy đơn của mình
                ViewBag.RecentOrders = _context.Orders
                    .Where(o => o.CreatedBy == userId)
                    .OrderByDescending(o => o.CreatedDate)
                    .Take(5)
                    .ToList();
            }
            else
            {
                // Admin: Thấy toàn bộ
                ViewBag.RecentOrders = _context.Orders
                    .OrderByDescending(o => o.CreatedDate)
                    .Take(5)
                    .ToList();
            }

            // 4. Sản phẩm sắp hết hàng (Cả 2 đều cần biết để nhập hàng/tư vấn)
            ViewBag.LowStockProducts = _context.Products
                .Where(p => p.Quantity < 10 && p.Status)
                .OrderBy(p => p.Quantity)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
