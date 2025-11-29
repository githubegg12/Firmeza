using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs;

/// <summary>
/// DTO for user registration request
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    /// <summary>User's first name</summary>
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es requerido")]
    /// <summary>User's last name</summary>
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    /// <summary>User's email address (used as username)</summary>
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento de identidad es requerido")]
    /// <summary>User's identification document number</summary>
    public string DocumentId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    /// <summary>User's contact phone number</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es requerida")]
    /// <summary>User's physical address</summary>
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    /// <summary>User's password</summary>
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    /// <summary>Password confirmation field</summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
