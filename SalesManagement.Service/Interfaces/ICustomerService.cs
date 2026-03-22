using SalesManagement.Data.Entities;

namespace SalesManagement.Service.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<bool> CreateCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> PhoneExistsAsync(string phone, int? excludeCustomerId = null);
        Task<IEnumerable<Order>> GetCustomerPurchaseHistoryAsync(int customerId);
    }
}
