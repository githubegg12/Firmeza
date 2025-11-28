using System.ComponentModel.DataAnnotations;

namespace Firmeza.web.Models;

/// <summary>
/// ViewModel for creating a new client in the admin panel.
/// Matches the fields required by the public registration form.
/// </summary>
public class CreateClientViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [Display(Name = "Nombre(s)")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [Display(Name = "Apellido(s)")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento de identidad es obligatorio")]
    [Display(Name = "Documento de Identidad")]
    public string DocumentId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [Display(Name = "Teléfono")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [Display(Name = "Dirección")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
