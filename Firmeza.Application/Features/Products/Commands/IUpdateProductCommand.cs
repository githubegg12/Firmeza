using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Products.Commands;

/// <summary>
/// Interface for updating product commands
/// </summary>
public interface IUpdateProductCommand
{
    Task<ProductDto> ExecuteAsync(int id, UpdateProductDto dto);
}

