
namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a client or customer of the business.
/// </summary>
public class Client
{
    public int Id { get; set; } // Primary key
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty; // e.g. NIT, ID, DNI
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Link to Identity user
    public string? UserId { get; set; }

    // Navigation property - a client can have multiple sales
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}