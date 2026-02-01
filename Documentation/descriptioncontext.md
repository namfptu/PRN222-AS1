# Context Diagram (DFD Level 0)

## 1. Diagram Overview

Biểu đồ ngữ cảnh (Context Diagram) dưới đây mô tả tổng quan về **Sales Management System** và mối quan hệ tương tác với 4 tác nhân bên ngoài (External Entities).

```mermaid
contextDiagram
    System("Sales Management System")

    %% Actors
    Admin("Admin")
    ProdMgr("Product Manager")
    Sales("Sales Staff")
    Warehouse("Warehouse Manager")

    %% Relations
    Admin <-->|Employee Account Data<br/>Management Report Data| System
    
    ProdMgr <-->|Product Master Data<br/>Category Data| System
    
    Sales <-->|Sales Transaction Data<br/>Product Search Data<br/>Customer Info| System
    System -->|Sales Invoice| Sales
    
    Warehouse <-->|Import Transaction Data<br/>Supplier Data| System
    System -->|Low Stock Alerts| Warehouse
```

*Lưu ý: Nếu Mermaid không hiển thị đúng mũi tên 2 chiều, vui lòng xem mô tả chi tiết bên dưới.*

---

## 2. Danh sách Tác nhân (Actors)

| Actor | Vai trò | Mô tả |
| :--- | :--- | :--- |
| **Admin** | Quản trị viên | Người chịu trách nhiệm quản lý tài khoản người dùng và xem báo cáo tổng hợp. |
| **Product Manager** | Quản lý sản phẩm | Người chịu trách nhiệm quản lý dữ liệu nền tảng (Sản phẩm, Danh mục). |
| **Sales Staff** | Nhân viên bán hàng | Người trực tiếp bán hàng, xử lý đơn hàng và chăm sóc khách hàng. |
| **Warehouse Manager** | Thủ kho | Người chịu trách nhiệm nhập hàng và quản lý nhà cung cấp. |

---

## 3. Mô tả Luồng Dữ liệu (Data Flows)

### 3.1. Admin ↔ System
*   🔄 **Employee Account Data:**
    *   **Input:** Admin cung cấp thông tin thêm mới, cập nhật role, hoặc reset mật khẩu cho nhân viên.
    *   **Output:** Hệ thống trả về danh sách nhân viên và trạng thái cập nhật.
*   🔄 **Management Report Data:**
    *   **Input:** Admin chọn tiêu chí lọc (thời gian).
    *   **Output:** Hệ thống trả về biểu đồ và số liệu thống kê doanh thu.

### 3.2. Product Manager ↔ System
*   🔄 **Product Master Data:**
    *   **Input:** Thông tin sản phẩm mới, cập nhật giá, trạng thái kinh doanh.
    *   **Output:** Danh sách sản phẩm hiện có, thông báo lỗi hợp lệ dữ liệu.
*   🔄 **Category Data:**
    *   **Input:** Tên danh mục mới.
    *   **Output:** Cấu trúc danh mục hiện có.

### 3.3. Sales Staff ↔ System
*   🔄 **Sales Transaction Data:**
    *   **Input:** Chi tiết đơn hàng (Sản phẩm, Số lượng), lệnh thanh toán, ủy quyền hủy đơn.
    *   **Output:** Xác nhận đơn hàng, lịch sử đơn hàng.
*   🔄 **Product Search Data:**
    *   **Input:** Từ khóa tìm kiếm.
    *   **Output:** Kết quả tìm kiếm kèm **số lượng tồn kho khả dụng**.
*   🔄 **Customer Info:**
    *   **Input:** Thông tin khách hàng mới.
    *   **Output:** Lịch sử mua hàng của khách.
*   ➡️ **Sales Invoice:** (System ➔ Staff)
    *   **Output:** File hóa đơn bán hàng (để in cho khách).

### 3.4. Warehouse Manager ↔ System
*   🔄 **Import Transaction Data:**
    *   **Input:** Phiếu nhập kho (Nhà cung cấp, Sản phẩm, Giá vốn).
    *   **Output:** Tổng chi phí nhập, lịch sử nhập kho.
*   🔄 **Supplier Data:**
    *   **Input:** Thông tin nhà cung cấp.
    *   **Output:** Danh bạ nhà cung cấp.
*   ➡️ **Low Stock Alerts:** (System ➔ Warehouse)
    *   **Output:** Thông báo danh sách các sản phẩm sắp hết hàng (Low stock).
