using Microsoft.AspNetCore.Identity;

namespace Firmeza.web.Data;

/// <summary>
/// Utility class to seed initial roles into the database.
/// </summary>
public static class SeedRoles
{
    public static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Administrador", "Cliente" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Crea el rol si no existe
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}