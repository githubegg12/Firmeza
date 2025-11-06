namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a single product line in a sale.
/// </summary>
public class SaleDetail
{
    public int Id { get; set; } // Primary key
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;

    // Foreign key to Sale
    public int SaleId { get; set; }
    public Sale? Sale { get; set; }

    // Foreign key to Product
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}