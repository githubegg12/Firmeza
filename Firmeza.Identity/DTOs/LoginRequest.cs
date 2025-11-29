using System.ComponentModel.DataAnnotations;

namespace Firmeza.Identity.DTOs;

/// <summary>
/// DTO for user login request
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    /// <summary>User email</summary>
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    /// <summary>User password</summary>
    public string Password { get; set; } = string.Empty;
}
