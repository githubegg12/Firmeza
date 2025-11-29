using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Product.Interfaces;

/// <summary>
/// Interface for updating product commands
/// </summary>
public interface IUpdateProductCommand
{
    /// <summary>
    /// Executes the update of an existing product
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <param name="dto">Updated product data</param>
    /// <returns>The updated product DTO</returns>
    Task<ProductDto> ExecuteAsync(int id, UpdateProductDto dto);
}

