namespace Firmeza.Application.DTOs.Sale;

/// <summary>
/// DTO representing a line item in a completed sale
/// </summary>
public class SaleDetailDto
{
    /// <summary>Product identifier</summary>
    public int ProductId { get; set; }

    /// <summary>Name of the product at the time of sale</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>Quantity purchased</summary>
    public int Quantity { get; set; }

    /// <summary>Price per unit at the time of sale</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Total price for this line item</summary>
    public decimal Subtotal => Quantity * UnitPrice;
}