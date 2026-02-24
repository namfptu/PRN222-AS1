using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagement.Data.Entities
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Role: 1 = Admin, 2 = Sales, 3 = ProductManager, 4 = Warehouse
        /// </summary>
        [Required]
        public int Role { get; set; }

        /// <summary>
        /// IsActive: true = Active, false = Inactive
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Collection of Import Orders created by this account
        /// </summary>
        public virtual ICollection<ImportOrder> CreatedImportOrders { get; set; } = new List<ImportOrder>();

        /// <summary>
        /// Extended profile information (1-to-1)
        /// </summary>
        public virtual AccountProfile? Profile { get; set; }
    }

    /// <summary>
    /// Enum for Account Roles
    /// </summary>
    public enum AccountRole
    {
        Admin = 1,
        Sales = 2,
        ProductManager = 3,
        Warehouse = 4
    }
}
