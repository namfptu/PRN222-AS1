using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            // Business logic: check specific rules if needed
            // For now, simple pass-through
            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();
            return account;
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _accountRepository.Update(account);
            await _accountRepository.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account != null)
            {
                _accountRepository.Delete(account);
                await _accountRepository.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _accountRepository.EmailExistsAsync(email);
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var account = await _accountRepository.GetByIdAsync(userId);
            if (account != null)
            {
                // In production, remember to Hash the password here!
                account.Password = newPassword; 
                _accountRepository.Update(account);
                await _accountRepository.SaveChangesAsync();
            }
        }
    }
}
