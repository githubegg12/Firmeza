namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a sales transaction in the system.
/// Each sale is associated with a user and contains multiple sale details (line items).
/// </summary>
public class Sale
{
    /// <summary>
    /// Gets or sets the unique identifier for the sale.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the sale was created.
    /// </summary>
    public DateTime SaleDate { get; set; }
    
    /// <summary>
    /// Gets or sets the user ID who made the purchase.
    /// References the AspNetUsers table (ApplicationUser).
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the navigation property to the user who made the purchase.
    /// </summary>
    public ApplicationUser? User { get; set; }
    
    /// <summary>
    /// Gets or sets the total amount of the sale (sum of all line items).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the file path or URL to the generated PDF receipt.
    /// This field is optional and populated after PDF generation.
    /// </summary>
    public string? ReceiptUrl { get; set; }

    /// <summary>
    /// Gets or sets the collection of line items (products) included in this sale.
    /// Navigation property for Entity Framework Core.
    /// </summary>
    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
