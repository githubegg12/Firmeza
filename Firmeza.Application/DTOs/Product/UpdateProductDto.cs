using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs;

/// <summary>
/// DTO for updating an existing product.
/// </summary>
public class UpdateProductDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public string? ImageUrl { get; set; }
}