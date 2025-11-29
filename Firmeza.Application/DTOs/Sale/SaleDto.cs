namespace Firmeza.Application.DTOs.Sale;

/// <summary>
/// DTO representing a completed sale transaction
/// </summary>
public class SaleDto
{
    /// <summary>Unique sale identifier</summary>
    public int Id { get; set; }

    /// <summary>ID of the user who made the purchase</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Full name of the user</summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>Email of the user</summary>
    public string UserEmail { get; set; } = string.Empty;

    /// <summary>List of items purchased</summary>
    public List<SaleDetailDto> Items { get; set; } = new();

    /// <summary>Total amount of the sale</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Date and time of the sale</summary>
    public DateTime SaleDate { get; set; }
    
    /// <summary>URL to the receipt PDF if available</summary>
    public string? ReceiptUrl { get; set; }
}