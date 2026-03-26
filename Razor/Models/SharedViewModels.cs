using System.ComponentModel.DataAnnotations;

namespace Razor.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }

    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }

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

        public string? Avatar { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

    // Customer Purchase History
    public class CustomerPurchaseHistoryViewModel
    {
        public SalesManagement.Data.Entities.Customer Customer { get; set; } = new();
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public List<CustomerOrderHistoryItemViewModel> Orders { get; set; } = new();
    }

    public class CustomerOrderHistoryItemViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public SalesManagement.Data.Entities.OrderStatus Status { get; set; }
        public string? Note { get; set; }
    }

    // User management ViewModels
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public int RoleId { get; set; }
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = string.Empty;
    }

    // Order ViewModels
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int? CustomerId { get; set; }

        public string? Note { get; set; }

        public List<CreateOrderItemViewModel> Items { get; set; } = new();
    }

    public class CreateOrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int AvailableQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int Quantity { get; set; }
    }

    // ImportOrder ViewModels
    public class CreateImportOrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Nhà cung cấp")]
        public int SupplierId { get; set; }

        public string? Note { get; set; }

        public List<ImportOrderDetailViewModel> Details { get; set; } = new();
    }

    public class ImportOrderDetailViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
