using Firmeza.Application.Interfaces; // Will use the interface from the Application layer
using Firmeza.Domain.Entities;
using Firmeza.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // CORRECTED: Using the correct 'User' entity
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, // CORRECTED
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            // Ejecutar migraciones pendientes
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }

            // Crear roles por defecto
            var roles = new[] { "Administrador", "Cliente", "Empleado" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear usuario administrador de prueba
            var adminEmail = "admin@firmeza.com";
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }

            // Crear usuario cliente de prueba
            var clientEmail = "cliente@firmeza.com";
            if (await _userManager.FindByEmailAsync(clientEmail) == null)
            {
                var clientUser = new ApplicationUser
                {
                    UserName = "cliente",
                    Email = clientEmail,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(clientUser, "Cliente123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(clientUser, "Cliente");
                }
            }

            // Crear datos de prueba si no existen
            // (Clients table removed, merged into Users)


            if (!_context.Products.Any())
            {
                var products = new[]
                {
                    new Product { Name = "Producto 1", Description = "Descripción 1", Category = "Categoría A", Price = 100.00m, Stock = 50, ImageUrl = "" },
                    new Product { Name = "Producto 2", Description = "Descripción 2", Category = "Categoría B", Price = 200.00m, Stock = 30, ImageUrl = "" },
                    new Product { Name = "Producto 3", Description = "Descripción 3", Category = "Categoría A", Price = 150.00m, Stock = 20, ImageUrl = "" }
                };
                _context.Products.AddRange(products);
                await _context.SaveChangesAsync();
            }
        }
    }
}
