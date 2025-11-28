namespace Firmeza.Application.DTOs.Sale;

public class SaleDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
    
    public string UserEmail { get; set; } = string.Empty;

    public List<SaleDetailDto> Items { get; set; } = new();

    public decimal TotalAmount { get; set; }

    public DateTime SaleDate { get; set; }
    
    public string? ReceiptUrl { get; set; }
}