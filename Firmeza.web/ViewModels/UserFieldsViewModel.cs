using System.ComponentModel.DataAnnotations;

namespace Firmeza.web.ViewModels;

public class UserFieldsViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string Password { get; set; } = string.Empty;

    public string? ConfirmPassword { get; set; }

    public bool IncludeEmail { get; set; }
    public bool IncludeConfirmPassword { get; set; }
}