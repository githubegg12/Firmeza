using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.Product.Interfaces;

/// <summary>
/// Service interface for product metrics and aggregations.
/// Follows SRP: Only responsible for dashboard metrics, not CRUD operations.
/// CRUD operations are handled by Commands/Queries (CQRS pattern).
/// </summary>
public interface IProductMetricsService
{
    /// <summary>
    /// Get total count of products
    /// </summary>
    Task<int> CountAsync();
    
    /// <summary>
    /// Calculate total inventory value (Price * Stock for all products)
    /// </summary>
    Task<decimal> GetTotalInventoryValueAsync();
    
    /// <summary>
    /// Count products with stock below threshold
    /// </summary>
    Task<int> GetLowStockCountAsync(int threshold = 5);
}
