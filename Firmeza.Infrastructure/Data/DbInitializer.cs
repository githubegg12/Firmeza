using Firmeza.Application.Interfaces; // Will use the interface from the Application layer
using Firmeza.Domain.Entities;
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
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }

            if (!await _roleManager.RoleExistsAsync("Administrador"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrador"));
            }
            if (!await _roleManager.RoleExistsAsync("Cliente"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Cliente"));
            }

            var adminEmail = "admin@firmeza.com";
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser // CORRECTED: Using the correct 'User' entity
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123*");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }
        }
    }
}
