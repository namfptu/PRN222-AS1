# Prompt for Gemini: Database Design Documentation

## Context

Tôi đang làm dự án **Sales Management System** sử dụng ASP.NET Core MVC với Entity Framework Core (Code-First approach). Hệ thống quản lý bán hàng, nhập kho, khách hàng, và nhân viên.

## Request

Hãy giúp tôi tạo tài liệu **Database Design** cho báo cáo dự án, bao gồm:

1. **Entity-Relationship Diagram (ERD)**
   - Sử dụng Mermaid.js để tạo ERD
   - Thể hiện tất cả các entities và relationships
   - Phân biệt rõ One-to-One, One-to-Many, Many-to-Many

2. **Database Schema & Table Descriptions**
   - Liệt kê tất cả các bảng trong database
   - Mô tả chi tiết từng cột (tên, kiểu dữ liệu, constraints)
   - Giải thích mục đích và ý nghĩa của từng bảng
   - Nêu rõ các Foreign Keys và Indexes

## Technical Details

- **Database Context**: `SalesManagementDbContext`
- **Database Provider**: SQL Server
- **Authentication**: Custom Cookie Authentication (không dùng ASP.NET Core Identity)
- **Entities Location**: `g:\SalesManagementSystem\SalesManagement.Data\Entities`

## Entities trong hệ thống:

### Core Module (User Management)
- `Account`: Lưu thông tin đăng nhập và phân quyền
  - Properties: Id, Email, Password, FullName, Role (1=Admin, 2=Staff), IsActive
- `AccountProfile`: Thông tin mở rộng của Account (1-to-1 relationship)
  - Properties: AccountId, PhoneNumber, Address, Avatar, DateOfBirth, JoinDate

### Product Management
- `Category`: Danh mục sản phẩm
  - Properties: Id, Name, Description, Status
- `Product`: Sản phẩm
  - Properties: Id, Name, Code, Price, Quantity, ImageUrl, Description, CategoryId, Status

### Sales Module
- `Customer`: Khách hàng
  - Properties: Id, FullName, Phone, Email, Address, CreatedDate, Status
- `Order`: Đơn hàng
  - Properties: Id, Code, CreatedDate, TotalAmount, Status (Pending/Done/Cancelled), CustomerId (nullable), CreatedBy, CustomerPhone, CustomerName, Note
- `OrderDetail`: Chi tiết đơn hàng (Composite Key: OrderId + ProductId)
  - Properties: OrderId, ProductId, UnitPrice, Quantity

### Procurement Module
- `Supplier`: Nhà cung cấp
  - Properties: Id, CompanyName, ContactPhone, Email, Address, Status
- `ImportOrder`: Phiếu nhập kho
  - Properties: Id, Code, ImportDate, TotalCost, Status (Draft/Completed/Cancelled), SupplierId, CreatedBy, Note
- `ImportOrderDetail`: Chi tiết phiếu nhập
  - Properties: Id, ImportOrderId, ProductId, Quantity, UnitCost

## Key Relationships

1. **Account ↔ AccountProfile**: 1-to-1, Cascade Delete
2. **Category → Products**: 1-to-Many, Restrict Delete
3. **Customer → Orders**: 1-to-Many, SetNull on Delete
4. **Account → Orders**: 1-to-Many (CreatedBy), Restrict Delete
5. **Order → OrderDetails**: 1-to-Many, Cascade Delete
6. **Product → OrderDetails**: 1-to-Many, Restrict Delete
7. **Supplier → ImportOrders**: 1-to-Many, Restrict Delete
8. **Account → ImportOrders**: 1-to-Many (CreatedBy), Restrict Delete
9. **ImportOrder → ImportOrderDetails**: 1-to-Many, Cascade Delete
10. **Product → ImportOrderDetails**: 1-to-Many, Restrict Delete

## Unique Constraints

- `Account.Email` (UNIQUE)
- `Customer.Phone` (UNIQUE)
- `Product.Code` (UNIQUE)
- `Order.Code` (UNIQUE)
- `ImportOrder.Code` (UNIQUE)

## Output Format

Tạo file markdown với cấu trúc sau:

```markdown
# Database Design

## 1. Entity-Relationship Diagram (ERD)

[Mermaid ERD diagram]

## 2. Database Schema

### 2.1. User Management Schema

#### Table: Accounts
[Mô tả bảng, ý nghĩa, columns, constraints]

#### Table: AccountProfiles
[...]

### 2.2. Product Management Schema

[...]

### 2.3. Sales Module Schema

[...]

### 2.4. Procurement Module Schema

[...]

## 3. Relationship Summary Table

| From Table | To Table | Relationship Type | Foreign Key | Delete Behavior |
|------------|----------|-------------------|-------------|-----------------|
| ... | ... | ... | ... | ... |

## 4. Indexes and Performance Considerations

[Liệt kê các unique indexes và giải thích lý do]
```

## Notes

- Sử dụng tiếng Việt cho phần mô tả
- ERD diagram sử dụng Mermaid syntax
- Format chuyên nghiệp, phù hợp cho báo cáo học thuật
- Giải thích rõ ràng, dễ hiểu cho người đọc không chuyên về database
