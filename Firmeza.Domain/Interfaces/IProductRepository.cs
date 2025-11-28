using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces;

/// <summary>
/// Defines the contract for product data access operations.
/// This interface follows the Repository pattern and will be implemented in the Infrastructure layer.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves all products from the database asynchronously.
    /// </summary>
    /// <returns>A collection of all products.</returns>
    Task<IEnumerable<Product>> GetAllAsync();
    
    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The product ID to search for.</param>
    /// <returns>The product if found; otherwise, null.</returns>
    Task<Product?> GetByIdAsync(int id);
    
    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    /// <param name="product">The product entity to add.</param>
    Task AddAsync(Product product);
    
    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    /// <param name="product">The product entity with updated values.</param>
    Task UpdateAsync(Product product);
    
    /// <summary>
    /// Deletes a product from the database.
    /// </summary>
    /// <param name="product">The product entity to delete.</param>
    Task DeleteAsync(Product product);
}
