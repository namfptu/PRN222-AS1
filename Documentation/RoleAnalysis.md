# Bảng Phân Quyền (Permission Matrix)

Dưới đây là bảng phân quyền chi tiết cho hệ thống Sales Management với 4 vai trò chuyên biệt:

| **Module** | **Chức năng** | **Admin** | **ProductManager** | **Sales** | **Warehouse** | **Ghi chú** |
| :--- | :--- | :---: | :---: | :---: | :---: | :--- |
| **Authentication** | Login / Logout | ✅ | ✅ | ✅ | ✅ | |
| **Dashboard** | Xem thống kê | ✅ (Full) | ❌ | ✅ (Sales) | ❌ | Admin xem toàn bộ, Sales xem doanh số cá nhân. |
| **Account** | Quản lý tài khoản | ✅ | ❌ | ❌ | ❌ | Chỉ Admin quản lý user hệ thống. |
| **Category** | Quản lý Danh mục | 👁️ (View) | ✅ | 👁️ (View) | 👁️ (View) | Admin chỉ giám sát, ProductManager chịu trách nhiệm. |
| **Product** | Quản lý Sản phẩm | 👁️ (View) | ✅ | 👁️ (View) | 👁️ (View) | Admin chỉ giám sát. |
| **Customer** | Quản lý Khách hàng | 👁️ (View) | ❌ | ✅ | ❌ | Admin chỉ giám sát. |
| **Order** | Tạo đơn hàng (POS) | ❌ | ❌ | ✅ | ❌ | Chỉ Sales được tạo đơn. |
| | Xem danh sách đơn | 👁️ (View) | ❌ | ✅ | ❌ | Admin xem được toàn bộ đơn để báo cáo. |
| | Hủy / Xóa đơn | 👁️ (View) | ❌ | ⚠️ (Pending) | ❌ | Admin có quyền can thiệp hủy đơn sai sót. |
| **Supplier** | Nhà cung cấp | 👁️ (View) | ❌ | ❌ | ✅ | Admin chỉ giám sát. |
| **ImportOrder** | Nhập kho | 👁️ (View) | ❌ | ❌ | ✅ | Admin chỉ giám sát. |

## Chú thích:
- ✅ **Full Access:** Xem, Thêm, Sửa, Xóa.
- 👁️ **View Only:** Chỉ được xem danh sách và chi tiết.
- ❌ **No Access:** Không có quyền truy cập.
- ⚠️ **Restricted:** Có quyền nhưng bị giới hạn điều kiện.

---

## Phân tích Role

### 1. Admin (Quản trị viên)
- **Trách nhiệm:** Quản lý User, cấu hình hệ thống và can thiệp khi có sự cố (Hủy đơn đã chốt, Sửa dữ liệu sai).
- **Scope:** Toàn quyền.

### 2. ProductManager (Quản lý sản phẩm)
- **Trách nhiệm:** Xây dựng và duy trì danh mục sản phẩm chuẩn hóa.
- **Scope:** Category, Product.
- **Mục tiêu:** Đảm bảo thông tin sản phẩm (Giá, Ảnh, Mô tả) luôn chính xác để Sales bán hàng.

### 3. Sales (Nhân viên kinh doanh)
- **Trách nhiệm:** Tìm kiếm khách hàng, tạo đơn hàng và theo dõi doanh số.
- **Scope:** Customer, Order.
- **Mục tiêu:** Tối đa hóa doanh thu, chăm sóc khách hàng.

### 4. Warehouse (Thủ kho)
- **Trách nhiệm:** Quản lý nguồn hàng đầu vào, làm việc với nhà cung cấp.
- **Scope:** Supplier, ImportOrder.
- **Mục tiêu:** Đảm bảo hàng hóa đủ tồn kho, kiểm soát giá vốn nhập vào.
