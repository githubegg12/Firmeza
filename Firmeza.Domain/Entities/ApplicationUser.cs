using Microsoft.AspNetCore.Identity;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Custom user class that inherits from IdentityUser.
/// This allows adding custom properties to the user in the future.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}