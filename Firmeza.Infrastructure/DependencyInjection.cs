using Firmeza.Application.Features.Products.Commands;
using Firmeza.Application.Features.Products.Queries;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Interfaces;
using Firmeza.Identity.Entities;
using Firmeza.Identity.Services;
using Firmeza.Infrastructure.Data;
using Firmeza.Infrastructure.Repositories;
using Firmeza.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IBulkImportService = Firmeza.Application.Features.BulkImport.IBulkImportService;
using IdentityOptions = Firmeza.Identity.Configurations.IdentityOptions;
using IPdfService = Firmeza.Application.Features.Pdf.IPdfService;
using IEmailService = Firmeza.Application.Interfaces.IEmailService;

namespace Firmeza.Infrastructure;

public static class DependencyInjection
{
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
        // Configure Application Cookie
        // -------------------------
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
        });

        // -------------------------
        // Register Repositories
        // -------------------------
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        // -------------------------
        // Register Product Features (Commands & Queries)
        // -------------------------
        services.AddScoped<ICreateProductCommand, CreateProductCommand>();
        services.AddScoped<IUpdateProductCommand, UpdateProductCommand>();
        services.AddScoped<IDeleteProductCommand, DeleteProductCommand>();
        services.AddScoped<IGetProductsQuery, GetProductsQuery>();


        // -------------------------
        // Register Custom Services
        // -------------------------
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

