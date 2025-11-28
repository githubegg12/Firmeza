using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs.Sale;

public class CreateSaleDto
{
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe incluir al menos un producto")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
    public List<CreateSaleDetailDto> Items { get; set; } = new();
}