namespace Firmeza.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new product.
/// Contains all required information to create a product in the system.
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the product category.
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the URL to the product image.
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the initial stock quantity.
    /// </summary>
    public int Stock { get; set; }
}
