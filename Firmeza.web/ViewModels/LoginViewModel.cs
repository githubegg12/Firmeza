using System.ComponentModel.DataAnnotations;

namespace Firmeza.web.ViewModels;

/// <summary>
/// View model for user login
/// </summary>
public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; } = "/";
}