using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

// Interface for reading product data
public interface IReadProductService
{
    // Get all products
    Task<IEnumerable<ProductDto>> GetAllProducts();

    // Get a single product by its Id
    Task<ProductDto?> GetProductById(int id);
}