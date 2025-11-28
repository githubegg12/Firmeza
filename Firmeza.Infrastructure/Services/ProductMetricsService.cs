using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Product.Interfaces;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Infrastructure.Services;

/// <summary>
/// Product metrics service implementation for dashboard aggregations.
/// Follows SRP: Only handles metrics and statistics, not CRUD operations.
/// CRUD operations are handled by Commands/Queries in the Application layer.
/// </summary>
public class ProductMetricsService : IProductMetricsService
{
    private readonly IProductRepository _productRepository;

    public ProductMetricsService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get total count of products in the system
    /// </summary>
    public async Task<int> CountAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Count();
    }

    /// <summary>
    /// Calculate total inventory value (sum of Price * Stock for all products)
    /// </summary>
    public async Task<decimal> GetTotalInventoryValueAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Sum(p => p.Price * p.Stock);
    }

    /// <summary>
    /// Count products with stock below the specified threshold
    /// </summary>
    public async Task<int> GetLowStockCountAsync(int threshold = 5)
    {
        var products = await _productRepository.GetAllAsync();
        return products.Count(p => p.Stock < threshold);
    }
}

