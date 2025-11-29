namespace Firmeza.Identity.Configurations;

/// <summary>
/// Centralizes all ASP.NET Core Identity configuration
/// </summary>
public static class IdentityOptions
{
    /// <summary>
    /// Configures password options, SignIn settings, and other Identity parameters
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