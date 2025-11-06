namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a sale transaction made to a client.
/// </summary>
public class Sale
{
    public int Id { get; set; } // Primary key
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    // Foreign key to Client
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    // Navigation property - each sale has multiple details
    public ICollection<SaleDetail> Details { get; set; } = new List<SaleDetail>();
}