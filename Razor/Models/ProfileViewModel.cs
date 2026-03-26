using System.ComponentModel.DataAnnotations;

namespace SalesManagement.WebApp.Models
{
    public class ProfileViewModel
    {
        // Read-only info
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }

        // Editable info
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }
        
        // Avatar URL (optional for now)
        public string? Avatar { get; set; }
    }
}
