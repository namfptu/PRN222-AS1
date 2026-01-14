using Microsoft.EntityFrameworkCore;
using SalesManagement.Data.Entities;

namespace SalesManagement.Data
{
    public class SalesManagementDbContext : DbContext
    {
        public SalesManagementDbContext(DbContextOptions<SalesManagementDbContext> options) 
            : base(options)
        {
        }

        // ==============================
        // DbSets - Sales Module
        // ==============================
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // ==============================
        // DbSets - Procurement Module
        // ==============================
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ImportOrder> ImportOrders { get; set; }
        public DbSet<ImportOrderDetail> ImportOrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==============================
            // Configure Primary Keys
            // ==============================
            
            // Composite Primary Key for OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            // ==============================
            // Configure Relationships - Sales Module
            // ==============================
            
            // OrderDetail -> Order (Cascade Delete)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderDetail -> Product (Restrict Delete)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product -> Category (Restrict Delete)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order -> Customer (Optional, Set Null on Delete)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Order -> Account (CreatedBy, Restrict Delete)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.CreatedByAccount)
                .WithMany()
                .HasForeignKey(o => o.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ==============================
            // Configure Relationships - Procurement Module
            // ==============================

            // ImportOrder -> Supplier (Restrict Delete)
            modelBuilder.Entity<ImportOrder>()
                .HasOne(io => io.Supplier)
                .WithMany(s => s.ImportOrders)
                .HasForeignKey(io => io.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // ImportOrder -> Account (CreatedBy, Restrict Delete)
            modelBuilder.Entity<ImportOrder>()
                .HasOne(io => io.CreatedByAccount)
                .WithMany()
                .HasForeignKey(io => io.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ImportOrderDetail -> ImportOrder (Cascade Delete)
            modelBuilder.Entity<ImportOrderDetail>()
                .HasOne(iod => iod.ImportOrder)
                .WithMany(io => io.ImportOrderDetails)
                .HasForeignKey(iod => iod.ImportOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ImportOrderDetail -> Product (Restrict Delete)
            modelBuilder.Entity<ImportOrderDetail>()
                .HasOne(iod => iod.Product)
                .WithMany()
                .HasForeignKey(iod => iod.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==============================
            // Configure Unique Constraints
            // ==============================
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Code)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone)
                .IsUnique();

            modelBuilder.Entity<ImportOrder>()
                .HasIndex(io => io.Code)
                .IsUnique();

            // ==============================
            // Seed Data - Accounts
            // ==============================
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    Email = "admin@electronics.com",
                    Password = "admin123", // In production, this should be hashed
                    FullName = "System Administrator",
                    Role = (int)AccountRole.Admin,
                    IsActive = true
                },
                new Account
                {
                    Id = 2,
                    Email = "staff@electronics.com",
                    Password = "staff123",
                    FullName = "Staff Demo",
                    Role = (int)AccountRole.Staff,
                    IsActive = true
                }
            );

            // ==============================
            // Seed Data - Categories
            // ==============================
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Smartphones", Description = "Mobile phones and accessories", Status = true },
                new Category { Id = 2, Name = "Laptops", Description = "Notebooks and laptops", Status = true },
                new Category { Id = 3, Name = "Tablets", Description = "Tablets and e-readers", Status = true },
                new Category { Id = 4, Name = "Accessories", Description = "Electronics accessories", Status = true },
                new Category { Id = 5, Name = "Audio", Description = "Speakers, headphones, earbuds", Status = true }
            );

            // ==============================
            // Seed Data - Customers
            // ==============================
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FullName = "Nguyễn Văn A",
                    Phone = "0901234567",
                    Email = "nguyenvana@gmail.com",
                    Address = "123 Nguyễn Huệ, Q.1, TP.HCM",
                    Status = true
                },
                new Customer
                {
                    Id = 2,
                    FullName = "Trần Thị B",
                    Phone = "0902345678",
                    Email = "tranthib@gmail.com",
                    Address = "456 Lê Lợi, Q.3, TP.HCM",
                    Status = true
                }
            );

            // ==============================
            // Seed Data - Suppliers
            // ==============================
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier
                {
                    Id = 1,
                    CompanyName = "Apple Vietnam",
                    ContactPhone = "028-1234-5678",
                    Email = "supply@apple.vn",
                    Address = "Tòa nhà Bitexco, Q.1, TP.HCM",
                    Status = true
                },
                new Supplier
                {
                    Id = 2,
                    CompanyName = "Samsung Electronics Vietnam",
                    ContactPhone = "028-2345-6789",
                    Email = "supply@samsung.vn",
                    Address = "KCN Thái Nguyên, Thái Nguyên",
                    Status = true
                },
                new Supplier
                {
                    Id = 3,
                    CompanyName = "Sony Vietnam",
                    ContactPhone = "028-3456-7890",
                    Email = "supply@sony.vn",
                    Address = "Q.7, TP.HCM",
                    Status = true
                }
            );

            // ==============================
            // Seed Data - Products
            // ==============================
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15 Pro Max",
                    Code = "IP15PM",
                    Price = 32990000,
                    Quantity = 50,
                    ImageUrl = "/images/products/iphone15promax.jpg",
                    Description = "iPhone 15 Pro Max 256GB",
                    CategoryId = 1,
                    Status = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S24 Ultra",
                    Code = "SS24U",
                    Price = 29990000,
                    Quantity = 40,
                    ImageUrl = "/images/products/galaxys24ultra.jpg",
                    Description = "Samsung Galaxy S24 Ultra 256GB",
                    CategoryId = 1,
                    Status = true
                },
                new Product
                {
                    Id = 3,
                    Name = "MacBook Pro 14 M3",
                    Code = "MBP14M3",
                    Price = 49990000,
                    Quantity = 25,
                    ImageUrl = "/images/products/macbookpro14.jpg",
                    Description = "MacBook Pro 14 inch M3 chip",
                    CategoryId = 2,
                    Status = true
                },
                new Product
                {
                    Id = 4,
                    Name = "iPad Pro 12.9 M2",
                    Code = "IPADPRO12",
                    Price = 28990000,
                    Quantity = 30,
                    ImageUrl = "/images/products/ipadpro.jpg",
                    Description = "iPad Pro 12.9 inch M2 chip",
                    CategoryId = 3,
                    Status = true
                },
                new Product
                {
                    Id = 5,
                    Name = "AirPods Pro 2",
                    Code = "APP2",
                    Price = 5990000,
                    Quantity = 100,
                    ImageUrl = "/images/products/airpodspro2.jpg",
                    Description = "AirPods Pro thế hệ 2",
                    CategoryId = 5,
                    Status = true
                }
            );
        }
    }
}
