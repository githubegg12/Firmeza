namespace Firmeza.Application.DTOs.Sale;

public class SaleDto
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public List<SaleDetailDto> Items { get; set; } = new();

    public decimal TotalAmount { get; set; }

    public DateTime SaleDate { get; set; }
}