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
            // Chi hien thi khach hang con hoat dong vi thao tac xoa trong Sales la xoa mem.
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
            // Khach hang moi duoc kich hoat ngay de Sales co the tao don lien.
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

            // Xoa mem giu nguyen lich su don hang cu nhung an khach nay khoi danh sach chon moi.
            customer.Status = false;
            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PhoneExistsAsync(string phone, int? excludeCustomerId = null)
        {
            // Cat khoang trang truoc khi kiem tra de tranh lot rule trung so dien thoai.
            var normalizedPhone = phone.Trim();

            return await _customerRepository.GetQueryable()
                .AnyAsync(c => c.Phone == normalizedPhone
                    && (!excludeCustomerId.HasValue || c.Id != excludeCustomerId.Value));
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
        {
            // Email la truong tuy chon, nhung neu da nhap thi khong duoc trung voi khach hang khac.
            var normalizedEmail = email.Trim().ToLower();

            return await _customerRepository.GetQueryable()
                .AnyAsync(c => c.Email != null
                    && c.Email.ToLower() == normalizedEmail
                    && (!excludeCustomerId.HasValue || c.Id != excludeCustomerId.Value));
        }

        public async Task<IEnumerable<Order>> GetCustomerPurchaseHistoryAsync(int customerId)
        {
            // Sap xep moi nhat len truoc de Sales xem lich su mua hang gan day nhanh hon.
            return await _orderRepository.GetQueryable()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
}
