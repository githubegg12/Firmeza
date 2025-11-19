using System.ComponentModel.DataAnnotations;

namespace Firmeza.web.ViewModels;

/// <summary>
/// View model for client management operations
/// </summary>
public class ClientViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Document { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required]
    public string Address { get; set; } = string.Empty;
}