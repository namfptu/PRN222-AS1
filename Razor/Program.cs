using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Data;
using SalesManagement.Service.Implementations;
using SalesManagement.Service.Interfaces;

namespace Razor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });

            // Add Razor Pages with page-level authorization conventions
            builder.Services.AddRazorPages(options =>
            {
                // Protected folders (require login)
                options.Conventions.AuthorizeFolder("/Dashboard");
                options.Conventions.AuthorizeFolder("/Category");
                options.Conventions.AuthorizeFolder("/Product");
                options.Conventions.AuthorizeFolder("/Customer");
                options.Conventions.AuthorizeFolder("/Supplier");
                options.Conventions.AuthorizeFolder("/Order");
                options.Conventions.AuthorizeFolder("/ImportOrder");
                options.Conventions.AuthorizeFolder("/User");
                options.Conventions.AuthorizeFolder("/Report");
                options.Conventions.AuthorizeFolder("/Role");
                options.Conventions.AuthorizeFolder("/Auth/Profile");

                // Public pages
                options.Conventions.AllowAnonymousToPage("/Auth/Login");
                options.Conventions.AllowAnonymousToPage("/Auth/Logout");
                options.Conventions.AllowAnonymousToPage("/Auth/AccessDenied");
            });

            // EF Core
            builder.Services.AddDbContext<SalesManagementDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SalesManagement.Data")
                ));

            // Register Repositories
            builder.Services.AddScoped<SalesManagement.Repo.Interfaces.IAccountRepository, SalesManagement.Repo.Implementations.AccountRepository>();
            builder.Services.AddScoped<SalesManagement.Repo.Interfaces.ICategoryRepository, SalesManagement.Repo.Implementations.CategoryRepository>();
            builder.Services.AddScoped<SalesManagement.Repo.Interfaces.IProductRepository, SalesManagement.Repo.Implementations.ProductRepository>();
            builder.Services.AddScoped(typeof(SalesManagement.Repo.Interfaces.IGenericRepository<>), typeof(SalesManagement.Repo.Implementations.GenericRepository<>));

            // Register Services
            builder.Services.AddScoped<SalesManagement.Service.Interfaces.IAccountService, SalesManagement.Service.Implementations.AccountService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IImportOrderService, ImportOrderService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();

            // Seed database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<SalesManagementDbContext>();
                    await SeedData.InitializeAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            }

            // Configure HTTP pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            await app.RunAsync();
        }
    }
}
