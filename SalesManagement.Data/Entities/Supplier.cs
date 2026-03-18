using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagement.Data.Entities
{
    [Table("Suppliers")]
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(10, ErrorMessage = "Số điện thoại phải có đúng 10 số")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải là 10 chữ số")]
        public string ContactPhone { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Status: true = Active, false = Inactive
        /// </summary>
        public bool Status { get; set; } = true;
         
        // Navigation property
        public virtual ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();
        public virtual ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    }
}
