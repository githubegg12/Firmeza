using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Services;

// Service for updating existing products
    public class UpdateProductService : IUpdateProductService
    {
        private readonly IProductRepository _repo;

        // Constructor injection of repository
        public UpdateProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        // Update product by Id
        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _repo.GetByIdAsync(id); // Fetch product
            if (product == null) return null;           // Return null if not found

            // Update product fields
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Category = dto.Category;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;

            await _repo.UpdateAsync(product); // Update in repository
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