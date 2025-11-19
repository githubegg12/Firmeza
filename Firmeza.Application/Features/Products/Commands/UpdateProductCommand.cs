using Firmeza.Application.DTOs;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Features.Products.Commands;

/// <summary>
/// Implementation of update product command
/// </summary>
public class UpdateProductCommand : IUpdateProductCommand
{
    private readonly IProductRepository _repository;

    public UpdateProductCommand(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> ExecuteAsync(int id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        product.Name = dto.Name ?? product.Name;
        product.Description = dto.Description ?? product.Description;
        product.Category = dto.Category ?? product.Category;
        product.Price = dto.Price > 0 ? dto.Price : product.Price;
        product.Stock = dto.Stock >= 0 ? dto.Stock : product.Stock;
        product.ImageUrl = dto.ImageUrl ?? product.ImageUrl;

        await _repository.UpdateAsync(product);

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

