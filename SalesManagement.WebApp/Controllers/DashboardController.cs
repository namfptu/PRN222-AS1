using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data;

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
            // Thống kê cơ bản
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalCustomers = _context.Customers.Count();
            ViewBag.TotalOrders = _context.Orders.Count();
            ViewBag.TodayOrders = _context.Orders.Count(o => o.CreatedDate.Date == DateTime.Today);
            
            // Tổng doanh thu
            ViewBag.TotalRevenue = _context.Orders
                .Where(o => o.Status == Data.Entities.OrderStatus.Done)
                .Sum(o => o.TotalAmount);
            
            // Doanh thu hôm nay
            ViewBag.TodayRevenue = _context.Orders
                .Where(o => o.Status == Data.Entities.OrderStatus.Done && o.CreatedDate.Date == DateTime.Today)
                .Sum(o => o.TotalAmount);

            // Đơn hàng gần đây (5 đơn mới nhất)
            ViewBag.RecentOrders = _context.Orders
                .OrderByDescending(o => o.CreatedDate)
                .Take(5)
                .ToList();

            // Sản phẩm sắp hết hàng (quantity < 10)
            ViewBag.LowStockProducts = _context.Products
                .Where(p => p.Quantity < 10 && p.Status)
                .OrderBy(p => p.Quantity)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
