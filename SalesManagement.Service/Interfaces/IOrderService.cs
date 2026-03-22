using SalesManagement.Data.Entities;

namespace SalesManagement.Service.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> CreateOrderAsync(Order order, List<OrderDetail> details);
        Task<Order?> GetOrderByIdAsync(int id);
    }
}
