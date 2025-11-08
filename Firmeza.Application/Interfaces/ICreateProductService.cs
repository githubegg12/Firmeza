using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

// Interface for creating a new product
public interface ICreateProductService
{
    // Create a new product using a DTO
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
}