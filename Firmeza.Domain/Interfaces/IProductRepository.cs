using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces;

// Interface for product repository
/// <summary>
/// Interface that defines data access methods for Product entities.
/// This will be implemented in the Infrastructure layer.
/// </summary>
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
