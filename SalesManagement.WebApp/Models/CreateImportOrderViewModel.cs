using System.ComponentModel.DataAnnotations;

namespace SalesManagement.WebApp.Models
{
    public class CreateImportOrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Nhà cung cấp")]
        public int SupplierId { get; set; }

        public string? Note { get; set; }

        // Danh sách các sản phẩm được thêm vào phiếu nhập
        public List<ImportOrderDetailViewModel> Details { get; set; } = new List<ImportOrderDetailViewModel>();
    }

    public class ImportOrderDetailViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}