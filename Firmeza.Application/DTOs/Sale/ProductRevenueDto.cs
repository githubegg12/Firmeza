namespace Firmeza.Application.DTOs.Sale;

public class ProductRevenueDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal CurrentUnitPrice { get; set; }
    public decimal TotalRevenue { get; set; }
}
