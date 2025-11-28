using Firmeza.Application.DTOs.Sale;
using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Sale.Interfaces;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Infrastructure.Services;

/// <summary>
/// Sales service implementation for dashboard metrics
/// </summary>
public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    // Dashboard Metrics - IMPLEMENTED
    public async Task<int> CountAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        return sales.Count();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        return sales.Sum(s => s.TotalAmount);
    }

    public async Task<IEnumerable<ProductRevenueDto>> GetProductRevenueReportAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        
        // Flatten all sale details
        var allDetails = sales.SelectMany(s => s.SaleDetails);
        
        // Group by Product and aggregate
        var report = allDetails
            .GroupBy(d => d.ProductId)
            .Select(g => new ProductRevenueDto
            {
                ProductId = g.Key,
                ProductName = g.First().Product?.Name ?? "Unknown",
                CurrentUnitPrice = g.First().Product?.Price ?? 0, // Taking current price from product entity
                TotalQuantitySold = g.Sum(d => d.Quantity),
                TotalRevenue = g.Sum(d => d.Total) // Sum of (Quantity * UnitPrice at time of sale)
            })
            .OrderByDescending(r => r.TotalRevenue)
            .ToList();

        return report;
    }

    // CRUD Operations - NOT IMPLEMENTED YET (throw NotImplementedException)
    public Task<IEnumerable<SaleDto>> GetAllSalesAsync()
    {
        throw new NotImplementedException("Use SaleController for CRUD operations");
    }

    public Task<SaleDto?> GetSaleByIdAsync(int id)
    {
        throw new NotImplementedException("Use SaleController for CRUD operations");
    }

    public Task<SaleDto> CreateSaleAsync(CreateSaleDto dto)
    {
        throw new NotImplementedException("Use SaleController for CRUD operations");
    }

    public Task<SaleDto?> UpdateSaleAsync(int id, UpdateSaleDto dto)
    {
        throw new NotImplementedException("Use SaleController for CRUD operations");
    }

    public Task<bool> DeleteSaleAsync(int id)
    {
        throw new NotImplementedException("Use SaleController for CRUD operations");
    }
}
