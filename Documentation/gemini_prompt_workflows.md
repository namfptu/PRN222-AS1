# Prompt for Gemini: Business Workflows Documentation

## Context

Tôi đang xây dựng **Sales Management System** (ASP.NET Core MVC). Tôi cần tài liệu mô tả chi tiết 4 luồng nghiệp vụ cốt lõi (Core Business Flows) của hệ thống để đưa vào báo cáo đồ án.

## Request

Hãy đóng vai trò là **Senior Business Analyst** và viết tài liệu mô tả chi tiết cho 4 workflows sau:

### 1. Luồng Quản trị Nền tảng (Platform Management)
*   **Actors:** Admin, Product Manager.
*   **Mục tiêu:** Thiết lập tài khoản nhân viên và dữ liệu sản phẩm ban đầu.
*   **Các bước chính:** Tạo tài khoản -> Phân quyền -> Tạo danh mục -> Tạo sản phẩm mới.

### 2. Luồng Nhập kho (Procurement / Inbound)
*   **Actor:** Warehouse Manager.
*   **Mục tiêu:** Nhập hàng từ nhà cung cấp để tăng tồn kho.
*   **Các bước chính:** Quản lý NCC -> Tạo phiếu nhập (Import Order) -> Duyệt nhập -> Hệ thống tăng tồn kho.

### 3. Luồng Bán hàng (Sales / Outbound)
*   **Actor:** Sales Staff.
*   **Mục tiêu:** Bán hàng cho khách và thu doanh thu.
*   **Các bước chính:** Tìm kiếm SP (kiểm tra tồn kho) -> Tạo/Chọn khách hàng -> Tạo đơn hàng (Order) -> Thanh toán -> In hóa đơn -> Hệ thống trừ tồn kho.

### 4. Luồng Báo cáo (Reporting)
*   **Actor:** Admin.
*   **Mục tiêu:** Giám sát hiệu quả kinh doanh.
*   **Các bước chính:** Hệ thống tổng hợp số liệu (Nhập/Xuất) -> Hiển thị Dashboard -> Admin xem báo cáo doanh thu/lợi nhuận.

## Output Requirements

Với mỗi Workflow, hãy trình bày theo cấu trúc sau:

1.  **Tên Quy trình** & **Mục đích**.
2.  **Pre-conditions (Điều kiện tiên quyết):** (Ví dụ: Phải có NCC mới nhập được hàng).
3.  **Detailed Steps (Các bước thực hiện):** Mô tả cụ thể hành động của User và phản hồi của System.
4.  **Activity Diagram (Mermaid):**
    *   Sử dụng syntax `graph TD` hoặc `flowchart TD`.
    *   Thể hiện rõ các bước rẽ nhánh (nếu có, ví dụ: Hết hàng -> Báo lỗi).
5.  **Post-conditions (Kết quả sau quy trình):** (Ví dụ: Tồn kho tăng, Tiền tăng).

## Format Note

*   Sử dụng **Tiếng Việt**.
*   Văn phong chuyên nghiệp, logic.
*   Diagram code đặt trong block code mermaid.
