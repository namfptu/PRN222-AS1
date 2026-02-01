# Context Diagram (DFD Level 0)

## 1. Diagram Overview

Biểu đồ ngữ cảnh (Context Diagram) dưới đây mô tả tổng quan về **Sales Management System** và mối quan hệ tương tác với 4 tác nhân bên ngoài (External Entities), bao gồm các phân hệ nâng cao (Advanced Features).

```mermaid
contextDiagram
    System("Sales Management System")

    %% Actors
    Admin("Admin")
    ProdMgr("Product Manager")
    Sales("Sales Staff")
    Warehouse("Warehouse Manager")

    %% Relations - Admin
    Admin <-->|Employee Account Data<br/>Management Reports<br/>Promotion Data<br/>System Audit Logs| System

    %% Relations - Product Manager
    ProdMgr <-->|Product Data<br/>Category Data<br/>Pricing Strategy Data<br/>Bulk Import/Export Data| System

    %% Relations - Sales Staff
    Sales <-->|Sales Transaction Data<br/>Product Search Data<br/>Customer Info<br/>Return & Warranty Data| System
    System -->|Sales Invoice| Sales

    %% Relations - Warehouse Manager
    Warehouse <-->|Import Order Data<br/>Supplier Data<br/>Stock Adjustment Data<br/>Return to Supplier Data| System
    System -->|Low Stock Alerts| Warehouse
```

---

## 2. Danh sách Tác nhân (Actors)

| Actor | Vai trò | Mô tả |
| :--- | :--- | :--- |
| **Admin** | Quản trị viên | Người chịu trách nhiệm quản trị hệ thống, nhân sự, rủi ro và các hoạt động marketing. |
| **Product Manager** | Quản lý sản phẩm | Người chịu trách nhiệm về chiến lược hàng hóa, dữ liệu sản phẩm, và chính sách giá. |
| **Sales Staff** | Nhân viên kinh doanh | Người trực tiếp bán hàng, chăm sóc khách hàng và hỗ trợ hậu mãi. |
| **Warehouse Manager** | Thủ kho | Người chịu trách nhiệm về tài sản kho bãi, nhập hàng và quản lý nguồn cung. |

---

## 3. Mô tả Luồng Dữ liệu (Data Flows)

### 3.1. Admin ↔ System
*   **Employee Account Data (2 chiều):** Quản lý thông tin tài khoản, phân quyền, reset mật khẩu.
*   **Management Reports (2 chiều):** Lọc và xem các báo cáo tổng hợp, biểu đồ tăng trưởng doanh thu.
*   **Promotion Data (2 chiều):** Cấu hình chương trình khuyến mãi, mã giảm giá (Voucher) và xem hiệu quả marketing.
*   **System Audit Logs (2 chiều):** Tra cứu nhật ký hệ thống để giám sát các hành động quan trọng/rủi ro.

### 3.2. Product Manager ↔ System
*   **Product Data (2 chiều):** Quản lý thông tin chi tiết từng sản phẩm (Tên, Ảnh, Mô tả).
*   **Category Data (2 chiều):** Quản lý cấu trúc danh mục hàng hóa.
*   **Pricing Strategy Data (2 chiều):** Thiết lập chiến lược giá, lịch trình giảm giá (Markdown) và xem lịch sử biến động giá.
*   **Bulk Import/Export Data (2 chiều):** Nhập liệu sản phẩm hàng loạt (Excel) hoặc xuất dữ liệu để báo cáo.

### 3.3. Sales Staff ↔ System
*   **Sales Transaction Data (2 chiều):** Thực hiện giao dịch bán hàng, thanh toán và xử lý đơn hàng.
*   **Product Search Data (2 chiều):** Tìm kiếm thông tin sản phẩm và kiểm tra số lượng tồn kho khả dụng.
*   **Customer Info (2 chiều):** Quản lý hồ sơ khách hàng và xem lịch sử mua hàng.
*   **Return & Warranty Data (2 chiều):** Xử lý yêu cầu đổi trả, tra cứu thông tin bảo hành cho khách.
*   **Sales Invoice (1 chiều System ➔ Staff):** Xuất hóa đơn bán hàng để in ấn.

### 3.4. Warehouse Manager ↔ System
*   **Import Order Data (2 chiều):** Tạo và quản lý phiếu nhập hàng từ nhà cung cấp.
*   **Supplier Data (2 chiều):** Quản lý danh bạ đối tác/nhà cung cấp.
*   **Stock Adjustment Data (2 chiều):** Thực hiện kiểm kê kho thực tế và điều chỉnh chênh lệch.
*   **Return to Supplier Data (2 chiều):** Xử lý các phiếu xuất trả hàng lỗi về lại nhà cung cấp.
*   **Low Stock Alerts (1 chiều System ➔ Warehouse):** Nhận cảnh báo tự động về hàng hóa dưới mức tồn kho tối thiểu.
