using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs.Sale;

public class UpdateSaleDto
{
    public string? UserId { get; set; }

    [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
    public List<UpdateSaleDetailDto>? Items { get; set; }
    
    public decimal? TotalAmount { get; set; }
}