using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

/// <summary>
/// Service interface for product operations.
/// </summary>

public interface IProductService
{
    // CRUD
    Task<IEnumerable<ProductDto>> GetAllProducts();
    Task<ProductDto?> GetProductById(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int id);
    
    Task<int> CountAsync();
    Task<decimal> GetTotalInventoryValueAsync();
    Task<int> GetLowStockCountAsync(int threshold = 5);
}
