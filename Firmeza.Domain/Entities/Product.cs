namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a product or construction material available for sale.
/// This entity stores all product information including pricing, inventory, and categorization.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the detailed description of the product.
    /// This field is optional and can contain product specifications or features.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the product category (e.g., "Cement", "Tools", "Hardware").
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the URL to the product image.
    /// This field is optional and should contain a valid image URL.
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the current stock quantity available.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Gets or sets the collection of sale details that reference this product.
    /// Navigation property for Entity Framework Core.
    /// </summary>
    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}