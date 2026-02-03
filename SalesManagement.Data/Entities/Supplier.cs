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

        [Required]
        [StringLength(20)]
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
    }
}
