using System.ComponentModel.DataAnnotations;

namespace Firmeza.Identity.DTOs;

/// <summary>
/// DTO for user registration request
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Role to assign to the user (default: Cliente)
    /// </summary>
    public string Role { get; set; } = "Cliente";
}
