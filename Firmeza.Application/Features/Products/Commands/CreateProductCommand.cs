using Firmeza.Application.DTOs;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Features.Products.Commands;

/// <summary>
/// Implementation of create product command
/// </summary>
public class CreateProductCommand : ICreateProductCommand
{
    private readonly IProductRepository _repository;

    public CreateProductCommand(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> ExecuteAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl
        };

        await _repository.AddAsync(product);

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

