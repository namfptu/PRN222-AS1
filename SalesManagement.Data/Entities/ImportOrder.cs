using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagement.Data.Entities
{
    [Table("ImportOrders")]
    public class ImportOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Import Order Status: Draft, Completed, Cancelled
        /// </summary>
        [Required]
        public ImportOrderStatus Status { get; set; } = ImportOrderStatus.Draft;

        [StringLength(500)]
        public string? Note { get; set; }

        // Foreign Keys
        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual Account? CreatedByAccount { get; set; }

        public virtual ICollection<ImportOrderDetail> ImportOrderDetails { get; set; } = new List<ImportOrderDetail>();
    }

    /// <summary>
    /// Enum for Import Order Status
    /// </summary>
    public enum ImportOrderStatus
    {
        Draft = 0,
        Completed = 1,
        Cancelled = 2
    }
}
