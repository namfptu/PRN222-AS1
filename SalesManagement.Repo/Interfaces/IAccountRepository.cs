using SalesManagement.Data.Entities;

namespace SalesManagement.Repo.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }
}
