using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

/// <summary>
/// Service interface for product operations.
/// </summary>
public interface IProductService
{
    // READ
    Task<IEnumerable<ProductDto>> GetAllProducts();
    Task<ProductDto?> GetProductById(int id);

    // CREATE
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);

    // UPDATE
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);

    // DELETE
    Task<bool> DeleteProductAsync(int id);
}