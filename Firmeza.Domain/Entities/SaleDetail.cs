namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a single line item in a sale transaction.
/// Each sale detail links a product to a sale with quantity and pricing information.
/// </summary>
public class SaleDetail
{
    /// <summary>
    /// Gets or sets the unique identifier for the sale detail.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the quantity of the product purchased.
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Gets or sets the unit price of the product at the time of sale.
    /// This preserves historical pricing even if the product price changes later.
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Gets the total price for this line item (Quantity × UnitPrice).
    /// This is a calculated property and not stored in the database.
    /// </summary>
    public decimal Total => Quantity * UnitPrice;

    /// <summary>
    /// Gets or sets the foreign key to the parent sale.
    /// </summary>
    public int SaleId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the parent sale.
    /// </summary>
    public Sale? Sale { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the product.
    /// </summary>
    public int ProductId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the product.
    /// </summary>
    public Product? Product { get; set; }
}