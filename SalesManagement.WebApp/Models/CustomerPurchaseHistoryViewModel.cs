using SalesManagement.Data.Entities;

namespace SalesManagement.WebApp.Models
{
    public class CustomerPurchaseHistoryViewModel
    {
        public Customer Customer { get; set; } = new Customer();
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public List<CustomerOrderHistoryItemViewModel> Orders { get; set; } = new();
    }

    public class CustomerOrderHistoryItemViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? Note { get; set; }
    }
}
