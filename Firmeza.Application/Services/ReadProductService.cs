using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Services;

// Service for reading product data
public class ReadProductService : IReadProductService
{
    private readonly IProductRepository _repo;

    // Constructor injection of repository
    public ReadProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    // Get all products and map to DTOs
    public async Task<IEnumerable<ProductDto>> GetAllProducts()
    {
        var products = await _repo.GetAllAsync(); // Fetch all products
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

    // Get a single product by Id and map to DTO
    public async Task<ProductDto?> GetProductById(int id)
    {
        var product = await _repo.GetByIdAsync(id); // Fetch product by Id
        if (product == null) return null; // Return null if not found

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