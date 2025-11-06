using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // Traer todos los productos
    public async Task<IEnumerable<ProductDto>> GetAllProducts()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            ImageUrl = p.ImageUrl,
            Price = p.Price,
            Stock = p.Stock
        });
    }

    // Traer un producto por Id
    public async Task<ProductDto?> GetProductById(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}
