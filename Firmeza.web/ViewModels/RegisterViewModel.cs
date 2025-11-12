using System.ComponentModel.DataAnnotations;

namespace Firmeza.web.ViewModels;

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
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Opción para asignar rol en registros administrados (por defecto Cliente)
    public string? Role { get; set; }
}

