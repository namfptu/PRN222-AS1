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
        public DbSet<AccountProfile> AccountProfiles { get; set; }
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

            // Account -> AccountProfile (1-to-1 with Cascade Delete)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Profile)
                .WithOne(p => p.Account)
                .HasForeignKey<AccountProfile>(p => p.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data has been moved to SeedData.cs
            // Call SeedData.InitializeAsync(context) in Program.cs
        }
    }
}
