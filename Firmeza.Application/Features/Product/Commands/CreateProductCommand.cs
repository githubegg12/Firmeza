using Firmeza.Application.DTOs;
using ProductEntity = Firmeza.Domain.Entities.Product;
using Firmeza.Domain.Interfaces;
using Firmeza.Application.Features.Product.Interfaces;

namespace Firmeza.Application.Features.Product.Commands;

/// <summary>
/// Command handler for creating new products in the system.
/// Implements the Command pattern to encapsulate product creation logic.
/// </summary>
public class CreateProductCommand : ICreateProductCommand
{
    private readonly IProductRepository _repository;

    /// <summary>
    /// Initializes a new instance of the CreateProductCommand class.
    /// </summary>
    /// <param name="repository">The product repository for data access.</param>
    public CreateProductCommand(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Executes the product creation command asynchronously.
    /// </summary>
    /// <param name="dto">The data transfer object containing product information.</param>
    /// <returns>A ProductDto representing the newly created product with its assigned ID.</returns>
    public async Task<ProductDto> ExecuteAsync(CreateProductDto dto)
    {
        // Map DTO to domain entity
        var product = new ProductEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl
        };

        // Persist to database
        await _repository.AddAsync(product);

        // Map entity back to DTO for response
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl
        };
    }
}

