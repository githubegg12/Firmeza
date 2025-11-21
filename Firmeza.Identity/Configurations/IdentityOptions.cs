namespace Firmeza.Identity.Configurations;

/// <summary>
/// Centraliza toda la configuración relacionada con ASP.NET Core Identity
/// </summary>
public static class IdentityOptions
{
    /// <summary>
    /// Configura las opciones de contraseña, SignIn y otros parámetros de Identity
    /// </summary>
    public static void ConfigurePasswordOptions(Microsoft.AspNetCore.Identity.IdentityOptions options)
    {
        // SignIn options
        options.SignIn.RequireConfirmedAccount = false;

        // Password options
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;

        // Lockout options
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User options
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        options.User.RequireUniqueEmail = false;
    }
}