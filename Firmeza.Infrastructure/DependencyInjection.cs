using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Data;
using Firmeza.Infrastructure.Repositories;
using Firmeza.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        // Configure Identity with options
        // -------------------------
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // SignIn options
                options.SignIn.RequireConfirmedAccount = false;

                // Password options
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

                // Lockout, user, etc., can also be configured here if needed
            })
            .AddRoles<IdentityRole>() // Enable roles
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders(); // Important for password reset, email confirmation, etc.

        // -------------------------
        // Register repositories
        // -------------------------
        services.AddScoped<IProductRepository, ProductRepository>();

        // -------------------------
        // Register custom services
        // -------------------------
        services.AddScoped<IBulkImportService, BulkImportService>();
        services.AddScoped<IPdfService, PdfService>();

        // -------------------------
        // Register Authentication service
        // Encapsulates Identity logic for Application layer
        // -------------------------
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
