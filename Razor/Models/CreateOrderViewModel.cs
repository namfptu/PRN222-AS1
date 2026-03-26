using System.ComponentModel.DataAnnotations;

namespace SalesManagement.WebApp.Models
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int? CustomerId { get; set; }

        public string? Note { get; set; }

        public List<CreateOrderItemViewModel> Items { get; set; } = new();
    }

    public class CreateOrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int AvailableQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int Quantity { get; set; }
    }
}
