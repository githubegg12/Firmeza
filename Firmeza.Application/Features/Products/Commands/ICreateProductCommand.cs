using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Products.Commands;

/// <summary>
/// Interface for creating product commands
/// </summary>
public interface ICreateProductCommand
{
    Task<ProductDto> ExecuteAsync(CreateProductDto dto);
}

