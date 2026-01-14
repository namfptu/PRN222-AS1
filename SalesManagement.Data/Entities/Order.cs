using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagement.Data.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Customer phone for quick orders (when customer is not registered)
        /// </summary>
        [StringLength(20)]
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// Customer name for quick orders
        /// </summary>
        [StringLength(150)]
        public string? CustomerName { get; set; }

        /// <summary>
        /// Order Status: Pending, Done, Cancelled
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Foreign Keys
        /// <summary>
        /// Registered Customer (optional - for walk-in customers)
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Staff who created this order
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Optional notes for the order
        /// </summary>
        [StringLength(500)]
        public string? Note { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual Account? CreatedByAccount { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    /// <summary>
    /// Enum for Order Status
    /// </summary>
    public enum OrderStatus
    {
        Pending = 0,
        Done = 1,
        Cancelled = 2
    }
}
