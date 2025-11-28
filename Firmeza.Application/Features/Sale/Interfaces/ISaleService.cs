using Firmeza.Application.DTOs.Sale;


namespace Firmeza.Application.Features.Sale.Interfaces;

public interface ISaleService
{
    // CRUD
    Task<IEnumerable<SaleDto>> GetAllSalesAsync();
    Task<SaleDto?> GetSaleByIdAsync(int id);
    Task<SaleDto> CreateSaleAsync(CreateSaleDto dto);
    Task<SaleDto?> UpdateSaleAsync(int id, UpdateSaleDto dto);
    Task<bool> DeleteSaleAsync(int id);

    // DASHBOARD METRICS
    Task<int> CountAsync();               // Total sales count
    Task<decimal> GetTotalRevenueAsync(); // Total money/gains
    Task<IEnumerable<ProductRevenueDto>> GetProductRevenueReportAsync();
    
}