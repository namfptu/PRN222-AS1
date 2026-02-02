# Prompt for Gemini: ERD Chen's Notation Analysis

## Context

Tôi cần vẽ **Entity-Relationship Diagram (ERD)** theo chuẩn **Chen's Notation** cho dự án Sales Management System.
Tôi cần bạn phân tích các Entities trong file code và liệt kê chi tiết các thành phần để tôi vẽ.

## Request

Hãy đóng vai trò là **Data Architect** và phân tích cấu trúc dữ liệu sau đây để chuẩn bị vẽ ERD Chen's Notation:

### 1. Entities & Attributes (Thực thể & Thuộc tính)
Liệt kê từng Entity (hình chữ nhật) và các Attributes (hình oval) của nó.
Đánh dấu rõ:
*   **Key Attribute** (Khóa chính) - *Gạch chân*
*   **Composite Attribute** (Thuộc tính phức hợp) - *(Nếu có)*
*   **Derived Attribute** (Thuộc tính dẫn xuất) - *(Ví dụ: SubTotal = Price * Quantity, TotalAmount)*
*   **Multivalued Attribute** (Thuộc tính đa trị) - *(Nếu có)*

### 2. Relationships (Mối quan hệ)
Liệt kê các Relationship (hình thoi) giữa các Entities.
Với mỗi quan hệ, xác định:
*   **Tên quan hệ** (Động từ, ví dụ: "Places", "Contains").
*   **Cardinality Ratio** (Tỷ số lực lượng): 1:1, 1:N, M:N.
*   **Participation Constraint** (Ràng buộc tham gia): Partial (Một phần) hay Total (Toàn bộ/Bắt buộc - 2 vạch).

## System Entities Input

Dưới đây là cấu trúc các Class C# trong dự án:

1.  **Account** (User)
    *   PK: `Id`
    *   Attrs: `Email`, `Password`, `FullName`, `Role`, `IsActive`
2.  **AccountProfile** (Weak Entity?)
    *   PK: `Id`
    *   Attrs: `PhoneNumber`, `Address`, `Avatar`, `JoinDate`, `AccountId` (FK)
3.  **Category**
    *   PK: `Id`
    *   Attrs: `Name`, `Description`, `Status`
4.  **Product**
    *   PK: `Id`
    *   Attrs: `Name`, `Code`, `Price`, `Quantity`, `ImageUrl`, `Description`, `Status`, `CategoryId` (FK)
5.  **Customer**
    *   PK: `Id`
    *   Attrs: `FullName`, `Phone`, `Email`, `Address`, `Status`
6.  **Supplier**
    *   PK: `Id`
    *   Attrs: `CompanyName`, `ContactPhone`, `Email`, `Address`, `Status`
7.  **Order**
    *   PK: `Id`
    *   Attrs: `Code`, `CreatedDate`, `TotalAmount` (Derived?), `Status`, `Note`
    *   FKs: `CustomerId`, `CreatedBy` (Account)
8.  **OrderDetail** (Associative Entity / Relationship Attribute?)
    *   Keys: `OrderId`, `ProductId`
    *   Attrs: `UnitPrice`, `Quantity`, `SubTotal` (Derived)
9.  **ImportOrder**
    *   PK: `Id`
    *   Attrs: `Code`, `ImportDate`, `TotalCost`, `Status`
    *   FKs: `SupplierId`, `CreatedBy` (Account)
10. **ImportOrderDetail**
    *   Keys: `ImportOrderId`, `ProductId`
    *   Attrs: `UnitCost`, `Quantity`, `SubTotal` (Derived)

## Output Format Requirements

Hãy trình bày kết quả phân tích theo format sau để tôi dễ vẽ:

### A. Danh sách Thực thể (Entities)
*   **Customer**:
    *   Attributes: <u>Id</u>, FullName, Phone, Email...
*   **Order**:
    *   Attributes: <u>Id</u>, Code, CreatedDate...
    *   Derived: TotalAmount (tính từ OrderDetails)

### B. Danh sách Quan hệ (Relationships)

| Entity 1 | Relationship (Verb) | Entity 2 | Cardinality | Participation (E1 : E2) | Ghi chú |
| :--- | :---: | :--- | :---: | :--- | :--- |
| Customer | **Places** (Đặt hàng) | Order | 1 : N | Total : Total | Một khách có thể đặt nhiều đơn. Đơn phải thuộc về 1 khách (nếu khách vãng lai thì sao?). |
| Account | **Manages** | Order | 1 : N | Partial : Total | Staff tạo đơn. |
| Order | **Contains** | Product | M : N | Total : Total | Quan hệ nhiều-nhiều này có thuộc tính riêng (Quantity, Price) -> Chuyển thành Associative Entity hoặc Relationship Attribute. |

### C. Lưu ý đặc biệt cho Chen's Notation
*   Với quan hệ M:N giữa Order và Product, hãy hướng dẫn tôi cách vẽ **OrderDetail** trong Chen's (dùng hình thoi hay hình chữ nhật lồng?).
*   Xác định rõ các thuộc tính dẫn xuất (nét đứt).

## Tone
Chuyên nghiệp, phân tích sâu về cơ sở dữ liệu.
