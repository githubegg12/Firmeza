namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a product or construction material available for sale.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    // Navigation property - list of sale details that include this product
    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}