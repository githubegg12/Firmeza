using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Product.Interfaces;

/// <summary>
/// Interface for retrieving products queries
/// </summary>
public interface IGetProductQuery
{
    /// <summary>
    /// Retrieves all products from the database
    /// </summary>
    /// <returns>Collection of product DTOs</returns>
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();

    /// <summary>
    /// Retrieves a specific product by its ID
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <returns>Product DTO if found, null otherwise</returns>
    Task<ProductDto?> GetProductByIdAsync(int id);
}
