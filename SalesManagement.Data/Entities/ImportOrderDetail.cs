using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagement.Data.Entities
{
    [Table("ImportOrderDetails")]
    public class ImportOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ImportOrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// Unit cost at import time (Giá nhập)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        // Computed property for subtotal
        [NotMapped]
        public decimal SubTotal => UnitCost * Quantity;

        // Navigation properties
        [ForeignKey("ImportOrderId")]
        public virtual ImportOrder? ImportOrder { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
