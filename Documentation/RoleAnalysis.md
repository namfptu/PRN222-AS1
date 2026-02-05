# Bảng Phân Quyền (Permission Matrix)

Dưới đây là bảng phân quyền chi tiết cho hệ thống Sales Management:....

| **Module** | **Chức năng** | **Admin** | **Staff** | **Ghi chú** |
| :--- | :--- | :---: | :---: | :--- |
| **Authentication** | Login / Logout | ✅ | ✅ | |
| **Dashboard** | Xem thống kê | ✅ (Full) | ✅ (Limited) | Admin xem doanh thu/lợi nhuận. Staff xem đơn hàng cá nhân/doanh số bán. |
| **Account** | Quản lý tài khoản | ✅ | ❌ | Chỉ Admin tạo/khóa tài khoản nhân viên. |
| **Category** | Quản lý Danh mục | ✅ | 👁️ (View Only) | Staff chỉ xem để biết phân loại, không được sửa cấu trúc. |
| **Product** | Quản lý Sản phẩm | ✅ | 👁️ (View Only) | Staff xem giá/tồn kho để tư vấn. **Không được sửa giá**. |
| **Customer** | Quản lý Khách hàng | ✅ | ✅ | Staff cần thêm khách mới khi bán hàng. |
| **Order** | Tạo đơn hàng (POS) | ✅ | ✅ | Chức năng chính của Sales. |
| | Xem danh sách đơn | ✅ (All) | ✅ (Mine/All) | Staff có thể xem lịch sử đơn hàng. |
| | Hủy / Xóa đơn | ✅ | ⚠️ (Restricted) | Staff chỉ hủy được đơn "Chờ xử lý". Đơn đã chốt phải gọi Admin. |
| **Supplier** | Nhà cung cấp | ✅ | ❌ | Thông tin đối tác nhập hàng là bảo mật kinh doanh. |
| **ImportOrder** | Nhập kho | ✅ | ❌ | Chỉ Admin/Thủ kho được phép nhập hàng và chỉnh giá vốn. |

## Chú thích:
- ✅ **Full Access:** Xem, Thêm, Sửa, Xóa (nếu có).
- 👁️ **View Only:** Chỉ được xem danh sách và chi tiết. Không có nút Thêm/Sửa/Xóa.
- ❌ **No Access:** Không nhìn thấy trên Menu, truy cập trực tiếp URL sẽ báo lỗi (Access Denied).
- ⚠️ **Restricted:** Có quyền nhưng bị giới hạn điều kiện (ví dụ: chỉ sửa được đơn của mình tạo, hoặc chỉ hủy đơn chưa duyệt).

---

## Phân tích Role

### 1. Admin
- **Vai trò:** Quản lý hệ thống, quản lý rủi ro (hủy đơn, tài khoản) và quản lý kho/tiền (nhập hàng, nhà cung cấp).
- **Workload:** Tập trung vào các tác vụ quan trọng nhưng tần suất thấp hơn (Nhập kho, Quản lý user). Các tác vụ nhập liệu hàng ngày (Tạo đơn, thêm khách) đã được chia sẻ cho Staff.

### 2. Staff
- **Vai trò:** Bán hàng trực tiếp (Sales).
- **Quyền hạn:** 
    - Tập trung vào **Order** (Tạo đơn) và **Customer** (Quản lý khách).
    - Các thông tin khác (Sản phẩm, Danh mục) chỉ được phép xem để tư vấn, không được sửa đổi để tránh sai lệch kho/giá.
