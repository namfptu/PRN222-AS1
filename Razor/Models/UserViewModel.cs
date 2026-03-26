using System.ComponentModel.DataAnnotations;

namespace SalesManagement.WebApp.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } // Map to Email as UserName usually
        public string Email { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; } // Display name: Admin / Staff
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public int RoleId { get; set; } // 1=Admin, 2=Sales, 3=ProductManager, 4=Warehouse
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public int RoleId { get; set; }
        
        public bool IsActive { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
