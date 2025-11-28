using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Product.Interfaces;

/// <summary>
/// Interface for creating product commands
/// </summary>
public interface ICreateProductCommand
{
    Task<ProductDto> ExecuteAsync(CreateProductDto dto);
}

