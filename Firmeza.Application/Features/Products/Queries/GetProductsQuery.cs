using Firmeza.Application.DTOs;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Features.Products.Queries;

/// <summary>
/// Implementation of queries to retrieve products
/// </summary>
public class GetProductsQuery : IGetProductsQuery
{
    private readonly IProductRepository _repository;

    public GetProductsQuery(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl
        });
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

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

