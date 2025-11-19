using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Products.Queries;

/// <summary>
/// Interface for retrieving products queries
/// </summary>
public interface IGetProductsQuery
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
}

