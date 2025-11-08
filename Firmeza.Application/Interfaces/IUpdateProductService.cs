using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

// Interface for updating an existing product
public interface IUpdateProductService
{
    // Update a product by its Id using a DTO
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
}