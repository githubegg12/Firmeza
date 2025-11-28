using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs.Sale;

public class UpdateSaleDetailDto
{
    [Required(ErrorMessage = "El ID del producto es requerido")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Quantity { get; set; }
    
    public decimal? UnitPrice { get; set; }
}
