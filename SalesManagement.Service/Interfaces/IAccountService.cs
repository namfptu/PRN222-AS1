using SalesManagement.Data.Entities;

namespace SalesManagement.Service.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account?> GetAccountByIdAsync(int id);
        Task<Account> CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}
