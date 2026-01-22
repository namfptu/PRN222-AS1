using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagement.Data.Entities
{
    [Table("AccountProfiles")]
    public class AccountProfile
    {
        [Key]
        public int AccountId { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? Avatar { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;
    }
}
