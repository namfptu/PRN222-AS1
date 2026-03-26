using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;

namespace Razor.Pages.ImportOrders
{
    public class DetailsModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public DetailsModel(SalesManagementDbContext context) => _context = context;

        public ImportOrder? ImportOrder { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ImportOrder = await _context.ImportOrders
                .Include(io => io.Supplier)
                .Include(io => io.ImportOrderDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(io => io.Id == id);

            if (ImportOrder == null) return NotFound();
            return Page();
        }
    }
}
