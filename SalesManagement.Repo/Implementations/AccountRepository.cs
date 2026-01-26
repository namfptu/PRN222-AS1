using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;

namespace SalesManagement.Repo.Implementations
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(SalesManagementDbContext context) : base(context)
        {
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(a => a.Email == email);
        }
    }
}
