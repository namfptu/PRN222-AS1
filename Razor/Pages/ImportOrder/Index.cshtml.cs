using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;

namespace Razor.Pages.ImportOrders
{
    public class IndexModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public IndexModel(SalesManagementDbContext context) => _context = context;

        public IEnumerable<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();

        public async Task OnGetAsync() =>
            ImportOrders = await _context.ImportOrders.Include(io => io.Supplier)
                .OrderByDescending(io => io.ImportDate).ToListAsync();
    }
}
