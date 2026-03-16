# Phân tích Luồng Login & Logout - SalesManagementSystem

Tài liệu này mô tả chi tiết luồng đăng nhập và đăng xuất trong hệ thống, ánh xạ trực tiếp với các thành phần trong codebase.

## 1. Cấu hình xác thực (Authentication Configuration)

Hệ thống sử dụng **Cookie Authentication** cơ bản của ASP.NET Core.

- **File cấu hình:** [Program.cs](file:///g:/SalesManagementSystem/SalesManagement.WebApp/Program.cs)
- **Cơ chế:**
  - `builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(...)`: Đăng ký dịch vụ xác thực bằng Cookie.
  - `options.LoginPath = "/Auth/Login"`: Chỉ định trang đăng nhập.
  - `options.LogoutPath = "/Auth/Logout"`: Chỉ định trang đăng xuất.
  - `app.UseAuthentication()` và `app.UseAuthorization()`: Middleware xử lý xác thực và phân quyền cho mỗi request.

---

## 2. Luồng Đăng nhập (Login Flow)

Luồng đi qua các bước sau:

### Bước 1: Yêu cầu trang Login (GET)
- **Controller Action:** `AuthController.Login(string? returnUrl)`
- **Code:** [AuthController.cs:L23-L33](file:///g:/SalesManagementSystem/SalesManagement.WebApp/Controllers/AuthController.cs#L23-L33)
- **Xử lý:** Kiểm tra nếu user đã đăng nhập (`User.Identity.IsAuthenticated`) thì chuyển hướng về Dashboard. Ngược lại, hiển thị View Login.

### Bước 2: Gửi thông tin đăng nhập (POST)
- **Controller Action:** `AuthController.Login(LoginViewModel model, string? returnUrl)`
- **Code:** [AuthController.cs:L38-L92](file:///g:/SalesManagementSystem/SalesManagement.WebApp/Controllers/AuthController.cs#L38-L92)
- **Mapping logic:**
  1. **Validation:** Kiểm tra `ModelState.IsValid` dựa trên các Data Annotations trong [LoginViewModel.cs](file:///g:/SalesManagementSystem/SalesManagement.WebApp/Models/LoginViewModel.cs).
  2. **Tìm tài khoản:** Kiểm tra Email trong database qua `_context.Accounts` (Sử dụng trực tiếp DbContext).
  3. **Xác thực mật khẩu:** So sánh `account.Password == model.Password` (Hiện tại đang so sánh chuỗi thô - TODO: Hash mật khẩu).
  4. **Tạo Identity:**
     - Tạo list các `Claim` (Id, Name, Email, Role).
     - Ánh xạ Role từ Enum `AccountRole` sang String để ASP.NET hiểu.
  5. **Đăng nhập hệ thống:** Gọi `await HttpContext.SignInAsync(...)` để ghi đè Cookie xác thực vào trình duyệt.
  6. **Điều hướng:** Redirect về `returnUrl` (nếu có và hợp lệ) hoặc về `Dashboard`.

---

## 3. Luồng Đăng xuất (Logout Flow)

### Yêu cầu Logout (GET)
- **Controller Action:** `AuthController.Logout()`
- **Code:** [AuthController.cs:L95-L99](file:///g:/SalesManagementSystem/SalesManagement.WebApp/Controllers/AuthController.cs#L95-L99)
- **Xử lý:**
  1. Gọi `await HttpContext.SignOutAsync(...)`: Xóa Cookie xác thực của scheme hiện tại.
  2. Redirect về trang Login.

---

## 4. Ánh xạ các thành phần Codebase

| Thành phần | Đường dẫn file | Vai trò |
| :--- | :--- | :--- |
| **Model** | [LoginViewModel.cs](file:///g:/SalesManagementSystem/SalesManagement.WebApp/Models/LoginViewModel.cs) | Chứa dữ liệu input từ Form (Email, Password, RememberMe). |
| **View** | `Views/Auth/Login.cshtml` | Giao diện người dùng. |
| **Controller** | [AuthController.cs](file:///g:/SalesManagementSystem/SalesManagement.WebApp/Controllers/AuthController.cs) | Điều phối luồng, xử lý logic SignIn/SignOut. |
| **Data Layer** | [SalesManagementDbContext.cs](file:///g:/SalesManagementSystem/SalesManagement.Data/SalesManagementDbContext.cs) | Truy vấn thông tin tài khoản trực tiếp từ DB. |
| **Services** | [AccountService.cs](file:///g:/SalesManagementSystem/SalesManagement.Service/Implementations/AccountService.cs) | (Đã đăng ký nhưng AuthController chưa dùng cho Login) - Chứa logic nghiệp vụ về Account. |
| **Repositories** | [AccountRepository.cs](file:///g:/SalesManagementSystem/SalesManagement.Repo/Implementations/AccountRepository.cs) | (Đã đăng ký nhưng AuthController chưa dùng cho Login) - Layer giao tiếp DB cho thực thể Account. |

> [!NOTE]
> Hiện tại `AuthController` đang dùng trực tiếp `DbContext` để query account trong action `Login`. Theo kiến trúc dự án, có thể refactor để sử dụng `IAccountService` nhằm đảm bảo tính nhất quán.
