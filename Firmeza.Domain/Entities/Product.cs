namespace Firmeza.Domain.Entities;

// Product entity represents an item that can be sold
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
