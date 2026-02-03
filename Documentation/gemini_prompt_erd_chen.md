# Prompt for Gemini: ERD Chen's Notation Analysis (Strict)

## Context

Tôi đang vẽ **ERD theo chuẩn Chen's Notation** cho dự án Sales Management System.
Tôi cần bạn phân tích các Entities và Relationships dựa trên Code C# đã cung cấp, tuân thủ nghiêm ngặt các ký hiệu của Chen.

## Request

Hãy đóng vai trò là **Data Architect** và xuất ra bảng phân tích chi tiết để tôi vẽ.

### 1. Phân loại Entity (Strong vs Weak)
*   **Strong Entity (Thực thể mạnh):** Hình chữ nhật thường.
*   **Weak Entity (Thực thể yếu):** Hình chữ nhật lồng (Double Rectangle).
    *   *Gợi ý:* AccountProfile, OrderDetail, ImportOrderDetail.

### 2. Quan hệ & Xác định (Identifying Relationships)
*   Xác định rõ quan hệ nào là **Identifying Relationship** (Quan hệ xác định - dùng cho thực thể yếu).
    *   Ký hiệu: Hình thoi lồng (Double Diamond).
*   Xác định **Cardinality Ratio** (1:1, 1:N, M:N).

### 3. Ràng buộc tham gia (Participation Constraints)
*   **Total Level (Tham gia toàn bộ):** Ký hiệu 2 nét (Double Line).
    *   *Quy tắc:* Thực thể yếu LUÔN có Total Participation với chủ nhân của nó.
*   **Partial Level (Tham gia một phần):** Ký hiệu 1 nét (Single Line).

## System Data Structure

Dưới đây là cấu trúc chính thức từ Code C#:

### A. Core Entities
1.  **Account** (Strong)
    *   PK: `Id`
    *   Attrs: `Email`, `Password`, `FullName`, `Role`
2.  **Category** (Strong)
    *   PK: `Id`
    *   Attrs: `Name`
3.  **Product** (Strong)
    *   PK: `Id`
    *   Attrs: `Name`, `Price`, `Quantity`
    *   FK: `CategoryId`
4.  **Customer** (Strong)
    *   PK: `Id`
    *   Attrs: `Phone`, `Name`
5.  **Supplier** (Strong)
    *   PK: `Id`
    *   Attrs: `CompanyName`, `Address`

### B. Transactional Entities
6.  **Order** (Strong)
    *   PK: `Id`
    *   Attrs: `Code`, `TotalAmount`, `CreatedDate`
    *   FKs: `CreatedBy` (Account), `CustomerId` (Customer - Nullable)
7.  **ImportOrder** (Strong)
    *   PK: `Id`
    *   Attrs: `Code`, `TotalCost`
    *   FKs: `CreatedBy` (Account), `SupplierId` (Supplier)

### C. Weak Entities (Cần vẽ Double Rectangle)
8.  **AccountProfile** (Weak)
    *   Owner: `Account`
    *   PK: `AccountId` (Vừa là PK vừa là FK)
    *   Attrs: `Address`, `Avatar`
9.  **OrderDetail** (Weak / Associate)
    *   Owners: `Order` & `Product`
    *   PK: `(OrderId, ProductId)`
    *   Attrs: `Quantity`, `UnitPrice`, `SubTotal` (Derived)
10. **ImportOrderDetail** (Weak / Associate)
    *   Owners: `ImportOrder` & `Product`
    *   PK: `(ImportOrderId, ProductId)`
    *   Attrs: `Quantity`, `UnitCost`

## Output Format Requirements

Hãy điền vào bảng sau:

### Bảng 1: Danh sách Quan hệ (Relationships Table)

| Entity 1 (Chủ/Mạnh) | Quan hệ (Verb) | Loại Hình Thoi | Entity 2 (Phụ/Yếu) | Cardinality | Participation (E1 : E2) | Ghi chú Vẽ |
| :--- | :---: | :---: | :--- | :---: | :---: | :--- |
| **Nhóm 1: Quản trị** | | | | | |
| Account | Has | **Double** | AccountProfile | 1 : 1 | Partial : **Total** | Profile yếu, phụ thuộc hoàn toàn Account. |
| Account | Manages | Single | Order | 1 : N | Partial : **Total** | Một Order bắt buộc phải có người tạo. |
| Account | Processes | Single | ImportOrder | 1 : N | Partial : **Total** | Một phiếu nhập bắt buộc phải có người tạo. |
| **Nhóm 2: Sản phẩm** | | | | | |
| Category | Categorizes | Single | Product | 1 : N | Partial : **Total** | Sản phẩm bắt buộc thuộc danh mục. |
| **Nhóm 3: Bán hàng** | | | | | |
| Customer | Places | Single | Order | 1 : N | Partial : Partial | Order có thể là khách vãng lai (CustomerId null). |
| Order | Contains | **Double** | OrderDetail | 1 : N | Partial : **Total** | OrderDetail là thực thể yếu của Order. |
| Product | Included in | **Double** | OrderDetail | 1 : N | Partial : **Total** | OrderDetail xác định bởi cả Product. |
| **Nhóm 4: Nhập kho** | | | | | |
| Supplier | Supplies | Single | ImportOrder | 1 : N | Partial : **Total** | Phiếu nhập bắt buộc từ NCC. |
| ImportOrder | Has Items | **Double** | ImportOrderDetail | 1 : N | Partial : **Total** | Chi tiết nhập phụ thuộc phiếu nhập. |
| Product | Restocked in | **Double** | ImportOrderDetail | 1 : N | Partial : **Total** | Chi tiết nhập xác định bởi cả Product. |

### B. Danh sách Thuộc tính (Attributes Highlight)
*   Liệt kê các thuộc tính **Derived** (Nét đứt): Ví dụ `SubTotal`, `TotalAmount`?
*   Liệt kê các thuộc tính **Multivalued** (Nét đôi): (Nếu có).

## Tone
Chính xác tuyệt đối về ký hiệu Chen.
