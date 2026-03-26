using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;
using SalesManagement.Data;
using SalesManagement.Data.Entities;

namespace Razor.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public IndexModel(SalesManagementDbContext context) => _context = context;

        public IEnumerable<UserViewModel> Users { get; set; } = new List<UserViewModel>();

        public void OnGet()
        {
            Users = _context.Accounts.Select(a => new UserViewModel
            {
                Id = a.Id,
                Email = a.Email,
                FullName = a.FullName,
                RoleId = a.Role,
                RoleName = ((AccountRole)a.Role).ToString(),
                IsActive = a.IsActive
            }).ToList();
        }
    }
}
