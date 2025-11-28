namespace Firmeza.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    
    // UserId references AspNetUsers table (configured in DbContext)
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    
    public decimal TotalAmount { get; set; }

    // New property to store the path to the generated PDF receipt.
    public string? ReceiptUrl { get; set; }

    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
