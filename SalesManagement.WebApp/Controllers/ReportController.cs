using Microsoft.AspNetCore.Mvc;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var lowStockProducts = await _reportService.GetLowStockProductsAsync(10);
            return View(lowStockProducts);
        }
    }
}