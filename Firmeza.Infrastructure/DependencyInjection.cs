using Firmeza.Application.Features.Product.Commands;
using Firmeza.Application.Features.Product.Queries;
using Firmeza.Application.Features.Product.Interfaces;
using Firmeza.Application.Features.Sale.Interfaces;
using Firmeza.Application.Features.Email.Interfaces;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Identity.Services;
using Firmeza.Infrastructure.Data;
using Firmeza.Infrastructure.Repositories;
using Firmeza.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Firmeza.Application.Features.BulkImport;
using IdentityOptions = Firmeza.Identity.Configurations.IdentityOptions;
using Firmeza.Application.Features.Pdf.Interfaces;
using IEmailService = Firmeza.Application.Features.Email.Interfaces.IEmailService;

namespace Firmeza.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services
/// Centralizes all dependency injection configuration for the Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds all Infrastructure services to the dependency injection container
    /// Includes DbContext, Identity, Repositories, and custom services
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">Application configuration containing connection strings and settings</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // -------------------------
        // Configure DbContext
        // -------------------------
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // -------------------------
        // Configure Identity with centralized options
        // -------------------------
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                IdentityOptions.ConfigurePasswordOptions(options);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // -------------------------
        // Configure Application Cookie for web authentication
        // -------------------------
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true; // Extends cookie lifetime on each request
        });

        // -------------------------
        // Register Repositories (Data Access Layer)
        // -------------------------
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        // -------------------------
        // Register Product Features (Commands & Queries)
        // Using CQRS pattern for product operations
        // -------------------------
        services.AddScoped<ICreateProductCommand, CreateProductCommand>();
        services.AddScoped<IUpdateProductCommand, UpdateProductCommand>();
        services.AddScoped<IDeleteProductCommand, DeleteProductCommand>();
        services.AddScoped<IGetProductQuery, GetProductQuery>();


        // -------------------------
        // Register Custom Services (Business Logic Layer)
        // -------------------------
        services.AddScoped<IProductMetricsService, ProductMetricsService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IBulkImportService, BulkImportService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        // -------------------------
        // Register Authentication Service
        // -------------------------
        services.AddScoped<IAuthService, AuthService>();

        // -------------------------
        // Register Database Initializer (for domain data seeding)
        // -------------------------
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}

