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

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // Register custom services
        services.AddScoped<IBulkImportService, BulkImportService>();

        return services;
    }
}
