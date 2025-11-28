using Microsoft.AspNetCore.Identity;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents an application user with extended properties beyond the default IdentityUser.
/// This class extends ASP.NET Core Identity to include custom user information
/// such as full name, document ID, and address for both admin and client users.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the user's full name (first name + last name).
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's government-issued document ID (e.g., national ID, passport).
    /// This field is unique across all users.
    /// </summary>
    public string DocumentId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's physical address.
    /// </summary>
    public string Address { get; set; } = string.Empty;
}