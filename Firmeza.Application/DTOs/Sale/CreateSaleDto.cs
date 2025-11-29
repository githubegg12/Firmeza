using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs.Sale;

/// <summary>
/// DTO for creating a new sale transaction
/// </summary>
public class CreateSaleDto
{
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    /// <summary>ID of the user making the purchase</summary>
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe incluir al menos un producto")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
    /// <summary>List of products to purchase</summary>
    public List<CreateSaleDetailDto> Items { get; set; } = new();
}