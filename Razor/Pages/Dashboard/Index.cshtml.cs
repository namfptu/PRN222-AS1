using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using System.Security.Claims;

namespace Razor.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly SalesManagementDbContext _context;

        public IndexModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        // Sales
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public List<Order> RecentOrders { get; set; } = new();

        // Products
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public List<Product> LowStockProducts { get; set; } = new();

        // Warehouse
        public int TotalSuppliers { get; set; }
        public int TodayImports { get; set; }
        public decimal TotalImportsAmount { get; set; }
        public List<ImportOrder> RecentImports { get; set; } = new();

        public void OnGet()
        {
            var isAdmin = User.IsInRole("Admin");
            var isSales = User.IsInRole("Sales") || isAdmin;
            var isProduct = User.IsInRole("ProductManager") || isAdmin;
            var isWarehouse = User.IsInRole("Warehouse") || isAdmin;

            int userId = 0;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);

            if (isSales)
            {
                if (User.IsInRole("Sales")) // Own orders only
                {
                    TotalOrders = _context.Orders.Count(o => o.CreatedBy == userId);
                    TodayOrders = _context.Orders.Count(o => o.CreatedBy == userId && o.CreatedDate.Date == DateTime.Today);
                    TotalRevenue = _context.Orders.Where(o => o.CreatedBy == userId && o.Status == OrderStatus.Done).Sum(o => o.TotalAmount);
                    TodayRevenue = _context.Orders.Where(o => o.CreatedBy == userId && o.Status == OrderStatus.Done && o.CreatedDate.Date == DateTime.Today).Sum(o => o.TotalAmount);
                    RecentOrders = _context.Orders.Where(o => o.CreatedBy == userId).OrderByDescending(o => o.CreatedDate).Take(5).ToList();
                }
                else // Admin: all orders
                {
                    TotalOrders = _context.Orders.Count();
                    TodayOrders = _context.Orders.Count(o => o.CreatedDate.Date == DateTime.Today);
                    TotalRevenue = _context.Orders.Where(o => o.Status == OrderStatus.Done).Sum(o => o.TotalAmount);
                    TodayRevenue = _context.Orders.Where(o => o.Status == OrderStatus.Done && o.CreatedDate.Date == DateTime.Today).Sum(o => o.TotalAmount);
                    RecentOrders = _context.Orders.OrderByDescending(o => o.CreatedDate).Take(5).ToList();
                }
                TotalCustomers = _context.Customers.Count();
            }

            if (isProduct)
            {
                TotalProducts = _context.Products.Count();
                TotalCategories = _context.Categories.Count();
                LowStockProducts = _context.Products.Where(p => p.Quantity < 10 && p.Status).OrderBy(p => p.Quantity).Take(5).ToList();
            }

            if (isWarehouse)
            {
                TotalSuppliers = _context.Suppliers.Count();
                TodayImports = _context.ImportOrders.Count(io => io.ImportDate.Date == DateTime.Today);
                TotalImportsAmount = _context.ImportOrders.Sum(io => io.TotalCost);
                RecentImports = _context.ImportOrders.Include(io => io.Supplier).OrderByDescending(io => io.ImportDate).Take(5).ToList();
            }
        }
    }
}
