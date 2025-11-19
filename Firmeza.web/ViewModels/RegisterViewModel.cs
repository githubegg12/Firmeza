using System.ComponentModel.DataAnnotations;

namespace Firmeza.web.ViewModels;

/// <summary>
/// View model for user registration
/// </summary>
public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Optional role assignment (defaults to Cliente if not provided)
    /// </summary>
    public string? Role { get; set; }
}