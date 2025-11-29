using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs.Sale;

/// <summary>
/// DTO for a line item in a new sale
/// </summary>
public class CreateSaleDetailDto
{
    [Required(ErrorMessage = "El ID del producto es requerido")]
    /// <summary>ID of the product to purchase</summary>
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    /// <summary>Quantity to purchase</summary>
    public int Quantity { get; set; }
}
