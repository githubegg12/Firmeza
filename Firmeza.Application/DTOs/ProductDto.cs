namespace Firmeza.Application.DTOs;

// DTO for transferring product data
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
