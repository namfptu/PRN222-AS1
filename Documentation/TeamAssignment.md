# Phân Công Nhiệm Vụ (Team Assignment)

Tài liệu này quy định rõ trách nhiệm của 4 thành viên trong team phát triển hệ thống Sales Management. Phân chia dựa trên các Module chức năng để giảm thiểu xung đột code (Merge Conflict).

## 👥 Tổng Quan Team

*   **Nguyên tắc chung:**
    *   Tự tạo Branch riêng khi dev (vd: `feature/product-management`).
    *   Sử dụng chung `_AdminLayout` và các Helper có sẵn.
    *   Tuân thủ quy tắc phân quyền đã chốt ở `RoleAnalysis.md`.

---

## 📅 Chi Tiết Phân Công

### 👤 Thành viên 1 (Admin / Team Leader)
**Trách nhiệm:** Dựng khung dự án, quản lý người dùng và báo cáo tổng quan.
*   **Màn hình & Chức năng:**
    1.  **Authentication:**
        *   Login / Logout (Đăng nhập/Đăng xuất).
        *   Profile (Thông tin cá nhân, Đổi mật khẩu).
        *   Màn hình "Access Denied" (Chặn quyền).
    2.  **Account Management (Admin Only):**
        *   Danh sách tài khoản (CRUD).
        *   Reset mật khẩu cho nhân viên.
        *   Phân quyền (Role: Admin, ProductManager, Sales, Warehouse).
    3.  **Dashboard (Admin/Overview):**
        *   Hiển thị thông tin tổng quan của cả 3 phân hệ: Sales, Product, và Warehouse.
        *   Tích hợp bộ điều hướng Sidebar theo quyền truy cập.

### 👤 Thành viên 2 (ProductManager)
**Trách nhiệm:** Quản lý dữ liệu nền tảng (Sản phẩm) - Xương sống của hệ thống.
*   **Màn hình & Chức năng:**
    1.  **Category (Danh mục):**
        *   CRUD Danh mục sản phẩm.
    2.  **Product (Sản phẩm):**
        *   CRUD Sản phẩm (Tên, Giá, Ảnh, Mô tả...).
        *   Upload ảnh sản phẩm.
        *   Kiểm tra logic (Không xóa sản phẩm đã có đơn hàng -> Chuyển trạng thái ngưng bán).
    3.  **Giao diện Read-Only:**
        *   Làm màn hình `Index` (Danh sách) riêng cho Sales/Warehouse (chỉ xem, ẩn nút sửa/xóa).
    4.  **Dashboard (Product Area):**
        *   Hiển thị thống kê tổng quan Danh mục, Sản phẩm.
        *   Danh sách cảnh báo sản phẩm sắp hết hàng (Low Stock).

### 👤 Thành viên 3 (Sales)
**Trách nhiệm:** Quy trình bán hàng đầu ra (Output) - Quan trọng nhất cho Sales Staff.
*   **Màn hình & Chức năng:**
    1.  **Customer (Khách hàng):**
        *   CRUD Khách hàng.
        *   Lịch sử mua hàng của khách.
    2.  **Order (Đơn hàng - POS):**
        *   **Tạo đơn hàng mới (Quan trọng & Khó):** Chọn khách -> Chọn SP -> Nhập số lượng -> Tính tiền.
        *   Xem danh sách đơn hàng.
        *   Xem chi tiết đơn hàng (In hóa đơn - View).
    3.  **Xử lý đơn:**
        *   Cập nhật trạng thái (Đang giao -> Hoàn thành / Hủy).
        *   Được cấp toàn quyền thao tác (Create/Update/Delete) trên module Order và Customer, chặn Admin thao tác.
    4.  **Dashboard (Sales Area):**
        *   Thống kê Doanh thu (Hôm nay/Tổng), số lượng đơn hàng, số lượng khách hàng.
        *   Bảng thông tin Đơn hàng bán mới nhất.

### 👤 Thành viên 4 (Warehouse)
**Trách nhiệm:** Quy trình nhập hàng đầu vào (Input) và Đối tác.
*   **Màn hình & Chức năng:**
    1.  **Supplier (Nhà cung cấp):**
        *   CRUD Nhà cung cấp.
    2.  **Import Order (Nhập kho):**
        *   Tạo phiếu nhập: Chọn NCC -> Chọn SP -> Nhập giá vốn & Số lượng.
        *   **Cập nhật tồn kho:** Khi nhập hàng -> Tự động cộng số lượng vào bảng Product (Trigger hoặc Code logic).
        *   Tính toán tổng chi phí nhập hàng.
    3.  **Báo cáo kho (Optional):**
        *   Xem danh sách sản phẩm sắp hết hàng (Low stock) và có phím tắt tạo Phiếu Nhập.
        *   Toàn quyền CUD trên Supplier, ImportOrder và Report (Admin bị chặn thao tác).
    4.  **Dashboard (Warehouse Area):**
        *   Thống kê Nhà cung cấp, chi phí nhập hàng, số đơn nhập trong ngày.
        *   Bảng thông tin Đơn nhập kho mới nhất.

---

## 🛤️ Lộ Trình Tích Hợp (Integration Plan)

1.  **Giai đoạn 1 (Core):**
    *   TV1: Setup dự án, Database, Login.
    *   Các TV khác: Pull code về, tạo Controller trống cho module mình.
2.  **Giai đoạn 2 (CRUD Cơ bản):**
    *   TV2: Xong Product/Category.
    *   TV3: Xong Customer.
    *   TV4: Xong Supplier.
3.  **Giai đoạn 3 (Nghiệp vụ Phức tạp - Logic chéo):**
    *   TV3: Làm chức năng Bán hàng (Cần dữ liệu Product của TV2 và Customer của TV3).
    *   TV4: Làm chức năng Nhập hàng (Cần dữ liệu Product của TV2 và Supplier của TV4).
    *   TV1: Hoàn thiện Dashboard (Cần dữ liệu Order của TV3 và Import của TV4).

---

## ⚠️ Lưu ý Quan Trọng
*   **TV3 và TV4** phụ thuộc nhiều vào dữ liệu của **TV2 (Product)**. -> **TV2 cần chốt cấu trúc bảng Product sớm nhất.**
*   Logic **Cập nhật tồn kho** cần thống nhất: 
    *   Nhập hàng (TV4) -> Tồn kho TĂNG.
    *   Bán hàng (TV3) -> Tồn kho GIẢM.
