using SalesManagement.Data;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;

namespace SalesManagement.Repo.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(SalesManagementDbContext context) : base(context)
        {
        }
    }
}
