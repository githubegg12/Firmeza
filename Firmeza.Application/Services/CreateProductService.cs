using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Services;

// Service for creating new products
public class CreateProductService : ICreateProductService
{
    private readonly IProductRepository _repo;

    // Constructor injection of repository
    public CreateProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    // Create a new product
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,               // Set product name
            Description = dto.Description, // Set product description
            Category = dto.Category,       // Set product category
            Price = dto.Price,             // Set product price
            Stock = dto.Stock,             // Set product stock
            ImageUrl = dto.ImageUrl        // Set product image URL (optional)
        };

        await _repo.AddAsync(product); // Add product to repository
        // Note: SaveChangesAsync() should be called in UnitOfWork or higher layer

        // Map entity to DTO and return
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
