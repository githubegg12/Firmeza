using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Product.Interfaces;

/// <summary>
/// Interface for creating product commands
/// </summary>
public interface ICreateProductCommand
{
    /// <summary>
    /// Executes the creation of a new product
    /// </summary>
    /// <param name="dto">Product creation data</param>
    /// <returns>The created product DTO</returns>
    Task<ProductDto> ExecuteAsync(CreateProductDto dto);
}

