using Microsoft.EntityFrameworkCore;
using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Order> _orderRepository;

        public CustomerService(
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Order> orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _customerRepository.GetQueryable()
                .Where(c => c.Status)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateCustomerAsync(Customer customer)
        {
            customer.Status = true;

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            customer.Status = false;
            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PhoneExistsAsync(string phone, int? excludeCustomerId = null)
        {
            var normalizedPhone = phone.Trim();

            return await _customerRepository.GetQueryable()
                .AnyAsync(c => c.Phone == normalizedPhone
                    && (!excludeCustomerId.HasValue || c.Id != excludeCustomerId.Value));
        }

        public async Task<IEnumerable<Order>> GetCustomerPurchaseHistoryAsync(int customerId)
        {
            return await _orderRepository.GetQueryable()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
}
