using Microsoft.EntityFrameworkCore;
using SalesManagement.Data.Entities;

namespace SalesManagement.Data
{
    /// <summary>
    /// Provides seed data for the database.
    /// Call SeedData.Initialize(dbContext) in Program.cs after database migration.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Initialize database with seed data.
        /// This method checks if data already exists before seeding.
        /// </summary>
        public static async Task InitializeAsync(SalesManagementDbContext context)
        {
            // Check if database can connect (don't run migrations automatically)
            // Run 'dotnet ef database update' manually before starting the app
            if (!await context.Database.CanConnectAsync())
            {
                throw new InvalidOperationException(
                    "Cannot connect to database. Please run 'dotnet ef database update' first.");
            }
            
            // Seed in order of dependencies
            await SeedAccountsAsync(context);
            await SeedCategoriesAsync(context);
            await SeedSuppliersAsync(context);
            await SeedCustomersAsync(context);
            await SeedProductsAsync(context);
            
            // Save all changes
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Synchronous version of Initialize for non-async contexts
        /// </summary>
        public static void Initialize(SalesManagementDbContext context)
        {
            InitializeAsync(context).GetAwaiter().GetResult();
        }

        #region Seed Accounts
        private static async Task SeedAccountsAsync(SalesManagementDbContext context)
        {
            if (await context.Accounts.AnyAsync())
                return;

            var accounts = new List<Account>
            {
                new Account
                {
                    Email = "admin@electronics.com",
                    Password = "admin123", // TODO: Hash password in production
                    FullName = "System Administrator",
                    Role = (int)AccountRole.Admin,
                    IsActive = true
                },
                new Account
                {
                    Email = "sales@electronics.com",
                    Password = "sales123",
                    FullName = "Sales Newbie",
                    Role = (int)AccountRole.Sales,
                    IsActive = true
                },
                new Account
                {
                    Email = "product@electronics.com",
                    Password = "product123",
                    FullName = "Product Manager",
                    Role = (int)AccountRole.ProductManager,
                    IsActive = true
                },
                new Account
                {
                    Email = "warehouse@electronics.com",
                    Password = "warehouse123",
                    FullName = "Warehouse Keeper",
                    Role = (int)AccountRole.Warehouse,
                    IsActive = true
                }
            };

            await context.Accounts.AddRangeAsync(accounts);
        }
        #endregion

        #region Seed Categories
        private static async Task SeedCategoriesAsync(SalesManagementDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Smartphones", Description = "Mobile phones and accessories", Status = true },
                new Category { Name = "Laptops", Description = "Notebooks and laptops", Status = true },
                new Category { Name = "Tablets", Description = "Tablets and e-readers", Status = true },
                new Category { Name = "Accessories", Description = "Electronics accessories", Status = true },
                new Category { Name = "Audio", Description = "Speakers, headphones, earbuds", Status = true },
                new Category { Name = "Gaming", Description = "Gaming consoles and accessories", Status = true },
                new Category { Name = "Wearables", Description = "Smartwatches and fitness trackers", Status = true }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync(); // Save to get IDs for Products
        }
        #endregion

        #region Seed Suppliers
        private static async Task SeedSuppliersAsync(SalesManagementDbContext context)
        {
            if (await context.Suppliers.AnyAsync())
                return;

            var suppliers = new List<Supplier>
            {
                new Supplier
                {
                    CompanyName = "Apple Vietnam",
                    ContactPhone = "028-1234-5678",
                    Email = "supply@apple.vn",
                    Address = "Tòa nhà Bitexco, Q.1, TP.HCM",
                    Status = true
                },
                new Supplier
                {
                    CompanyName = "Samsung Electronics Vietnam",
                    ContactPhone = "028-2345-6789",
                    Email = "supply@samsung.vn",
                    Address = "KCN Thái Nguyên, Thái Nguyên",
                    Status = true
                },
                new Supplier
                {
                    CompanyName = "Sony Vietnam",
                    ContactPhone = "028-3456-7890",
                    Email = "supply@sony.vn",
                    Address = "Q.7, TP.HCM",
                    Status = true
                },
                new Supplier
                {
                    CompanyName = "Xiaomi Vietnam",
                    ContactPhone = "028-4567-8901",
                    Email = "supply@xiaomi.vn",
                    Address = "Q.2, TP.HCM",
                    Status = true
                },
                new Supplier
                {
                    CompanyName = "Dell Vietnam",
                    ContactPhone = "028-5678-9012",
                    Email = "supply@dell.vn",
                    Address = "Q.Bình Thạnh, TP.HCM",
                    Status = true
                }
            };

            await context.Suppliers.AddRangeAsync(suppliers);
        }
        #endregion

        #region Seed Customers
        private static async Task SeedCustomersAsync(SalesManagementDbContext context)
        {
            if (await context.Customers.AnyAsync())
                return;

            var customers = new List<Customer>
            {
                new Customer
                {
                    FullName = "Nguyễn Văn A",
                    Phone = "0901234567",
                    Email = "nguyenvana@gmail.com",
                    Address = "123 Nguyễn Huệ, Q.1, TP.HCM",
                    Status = true
                },
                new Customer
                {
                    FullName = "Trần Thị B",
                    Phone = "0902345678",
                    Email = "tranthib@gmail.com",
                    Address = "456 Lê Lợi, Q.3, TP.HCM",
                    Status = true
                },
                new Customer
                {
                    FullName = "Lê Văn C",
                    Phone = "0903456789",
                    Email = "levanc@gmail.com",
                    Address = "789 Trần Hưng Đạo, Q.5, TP.HCM",
                    Status = true
                },
                new Customer
                {
                    FullName = "Phạm Thị D",
                    Phone = "0904567890",
                    Email = "phamthid@gmail.com",
                    Address = "321 Cách Mạng Tháng 8, Q.10, TP.HCM",
                    Status = true
                },
                new Customer
                {
                    FullName = "Hoàng Văn E",
                    Phone = "0905678901",
                    Email = "hoangvane@gmail.com",
                    Address = "654 Hai Bà Trưng, Q.1, TP.HCM",
                    Status = true
                }
            };

            await context.Customers.AddRangeAsync(customers);
        }
        #endregion

        #region Seed Products
        private static async Task SeedProductsAsync(SalesManagementDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            // Get category IDs
            var smartphoneCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Smartphones");
            var laptopCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Laptops");
            var tabletCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Tablets");
            var audioCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Audio");
            var accessoriesCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Accessories");
            var wearablesCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Wearables");

            var products = new List<Product>();

            // Smartphones
            if (smartphoneCategory != null)
            {
                products.AddRange(new[]
                {
                    new Product
                    {
                        Name = "iPhone 15 Pro Max",
                        Code = "IP15PM",
                        Price = 32990000,
                        Quantity = 50,
                        ImageUrl = "/images/products/iphone15promax.jpg",
                        Description = "iPhone 15 Pro Max 256GB - Titanium Natural",
                        CategoryId = smartphoneCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy S24 Ultra",
                        Code = "SS24U",
                        Price = 29990000,
                        Quantity = 40,
                        ImageUrl = "/images/products/galaxys24ultra.jpg",
                        Description = "Samsung Galaxy S24 Ultra 256GB - Titanium Gray",
                        CategoryId = smartphoneCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "iPhone 15",
                        Code = "IP15",
                        Price = 22990000,
                        Quantity = 60,
                        ImageUrl = "/images/products/iphone15.jpg",
                        Description = "iPhone 15 128GB - Blue",
                        CategoryId = smartphoneCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Xiaomi 14 Ultra",
                        Code = "XI14U",
                        Price = 23990000,
                        Quantity = 35,
                        ImageUrl = "/images/products/xiaomi14ultra.jpg",
                        Description = "Xiaomi 14 Ultra 512GB - Black",
                        CategoryId = smartphoneCategory.Id,
                        Status = true
                    }
                });
            }

            // Laptops
            if (laptopCategory != null)
            {
                products.AddRange(new[]
                {
                    new Product
                    {
                        Name = "MacBook Pro 14 M3",
                        Code = "MBP14M3",
                        Price = 49990000,
                        Quantity = 25,
                        ImageUrl = "/images/products/macbookpro14.jpg",
                        Description = "MacBook Pro 14 inch M3 Pro chip - Space Black",
                        CategoryId = laptopCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "MacBook Air 15 M3",
                        Code = "MBA15M3",
                        Price = 32990000,
                        Quantity = 30,
                        ImageUrl = "/images/products/macbookair15.jpg",
                        Description = "MacBook Air 15 inch M3 chip - Midnight",
                        CategoryId = laptopCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Dell XPS 15",
                        Code = "DXPS15",
                        Price = 45990000,
                        Quantity = 20,
                        ImageUrl = "/images/products/dellxps15.jpg",
                        Description = "Dell XPS 15 Intel i7-13700H, 32GB RAM, 1TB SSD",
                        CategoryId = laptopCategory.Id,
                        Status = true
                    }
                });
            }

            // Tablets
            if (tabletCategory != null)
            {
                products.AddRange(new[]
                {
                    new Product
                    {
                        Name = "iPad Pro 12.9 M2",
                        Code = "IPADPRO12",
                        Price = 28990000,
                        Quantity = 30,
                        ImageUrl = "/images/products/ipadpro.jpg",
                        Description = "iPad Pro 12.9 inch M2 chip 256GB WiFi",
                        CategoryId = tabletCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "iPad Air 5",
                        Code = "IPADAIR5",
                        Price = 16990000,
                        Quantity = 45,
                        ImageUrl = "/images/products/ipadair5.jpg",
                        Description = "iPad Air 5 M1 chip 64GB WiFi - Space Gray",
                        CategoryId = tabletCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy Tab S9 Ultra",
                        Code = "TABS9U",
                        Price = 27990000,
                        Quantity = 25,
                        ImageUrl = "/images/products/tabs9ultra.jpg",
                        Description = "Samsung Galaxy Tab S9 Ultra 256GB WiFi",
                        CategoryId = tabletCategory.Id,
                        Status = true
                    }
                });
            }

            // Audio
            if (audioCategory != null)
            {
                products.AddRange(new[]
                {
                    new Product
                    {
                        Name = "AirPods Pro 2",
                        Code = "APP2",
                        Price = 5990000,
                        Quantity = 100,
                        ImageUrl = "/images/products/airpodspro2.jpg",
                        Description = "AirPods Pro thế hệ 2 với USB-C",
                        CategoryId = audioCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Sony WH-1000XM5",
                        Code = "SNWH1000XM5",
                        Price = 8490000,
                        Quantity = 40,
                        ImageUrl = "/images/products/sonywh1000xm5.jpg",
                        Description = "Sony WH-1000XM5 Wireless Noise Cancelling",
                        CategoryId = audioCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "AirPods Max",
                        Code = "APM",
                        Price = 12490000,
                        Quantity = 20,
                        ImageUrl = "/images/products/airpodsmax.jpg",
                        Description = "AirPods Max - Silver",
                        CategoryId = audioCategory.Id,
                        Status = true
                    }
                });
            }

            // Wearables
            if (wearablesCategory != null)
            {
                products.AddRange(new[]
                {
                    new Product
                    {
                        Name = "Apple Watch Ultra 2",
                        Code = "AWU2",
                        Price = 21990000,
                        Quantity = 25,
                        ImageUrl = "/images/products/applewatchultra2.jpg",
                        Description = "Apple Watch Ultra 2 GPS + Cellular 49mm",
                        CategoryId = wearablesCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Apple Watch Series 9",
                        Code = "AWS9",
                        Price = 10990000,
                        Quantity = 50,
                        ImageUrl = "/images/products/applewatchs9.jpg",
                        Description = "Apple Watch Series 9 GPS 45mm",
                        CategoryId = wearablesCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy Watch 6 Classic",
                        Code = "SGW6C",
                        Price = 9990000,
                        Quantity = 35,
                        ImageUrl = "/images/products/galaxywatch6.jpg",
                        Description = "Samsung Galaxy Watch 6 Classic 47mm",
                        CategoryId = wearablesCategory.Id,
                        Status = true
                    }
                });
            }

            // Accessories
            if (accessoriesCategory != null)
            {
                products.AddRange(new[]
                {
                    new Product
                    {
                        Name = "Apple Magic Keyboard",
                        Code = "AMK",
                        Price = 7990000,
                        Quantity = 30,
                        ImageUrl = "/images/products/magickeyboard.jpg",
                        Description = "Apple Magic Keyboard for iPad Pro 12.9",
                        CategoryId = accessoriesCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "Apple Pencil 2",
                        Code = "AP2",
                        Price = 3490000,
                        Quantity = 60,
                        ImageUrl = "/images/products/applepencil2.jpg",
                        Description = "Apple Pencil thế hệ 2",
                        CategoryId = accessoriesCategory.Id,
                        Status = true
                    },
                    new Product
                    {
                        Name = "MagSafe Charger",
                        Code = "MSC",
                        Price = 1090000,
                        Quantity = 80,
                        ImageUrl = "/images/products/magsafe.jpg",
                        Description = "Apple MagSafe Charger",
                        CategoryId = accessoriesCategory.Id,
                        Status = true
                    }
                });
            }

            if (products.Any())
            {
                await context.Products.AddRangeAsync(products);
            }
        }
        #endregion

        #region Force Reseed (Delete all and reseed)
        /// <summary>
        /// WARNING: This will delete ALL existing data and reseed.
        /// Use only for development/testing purposes.
        /// </summary>
        public static async Task ForceReseedAsync(SalesManagementDbContext context)
        {
            // Delete in reverse order of dependencies
            context.OrderDetails.RemoveRange(context.OrderDetails);
            context.Orders.RemoveRange(context.Orders);
            context.ImportOrderDetails.RemoveRange(context.ImportOrderDetails);
            context.ImportOrders.RemoveRange(context.ImportOrders);
            context.Products.RemoveRange(context.Products);
            context.Customers.RemoveRange(context.Customers);
            context.Suppliers.RemoveRange(context.Suppliers);
            context.Categories.RemoveRange(context.Categories);
            context.Accounts.RemoveRange(context.Accounts);
            
            await context.SaveChangesAsync();

            // Reset identity seeds if using SQL Server
            try
            {
                await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Accounts', RESEED, 0)");
                await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Categories', RESEED, 0)");
                await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Suppliers', RESEED, 0)");
                await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Customers', RESEED, 0)");
                await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0)");
                await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Orders', RESEED, 0)");
                await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('ImportOrders', RESEED, 0)");
            }
            catch
            {
                // Ignore if not using SQL Server or tables don't exist
            }

            // Reseed
            await SeedAccountsAsync(context);
            await SeedCategoriesAsync(context);
            await SeedSuppliersAsync(context);
            await SeedCustomersAsync(context);
            await SeedProductsAsync(context);
            
            await context.SaveChangesAsync();
        }
        #endregion
    }
}
