using Firmeza.Application.DTOs.Sale;


namespace Firmeza.Application.Features.Sale.Interfaces;

public interface ISaleService
{
    // CRUD Operations
    
    /// <summary>
    /// Retrieves all sales
    /// </summary>
    Task<IEnumerable<SaleDto>> GetAllSalesAsync();

    /// <summary>
    /// Retrieves a specific sale by ID
    /// </summary>
    Task<SaleDto?> GetSaleByIdAsync(int id);

    /// <summary>
    /// Creates a new sale
    /// </summary>
    Task<SaleDto> CreateSaleAsync(CreateSaleDto dto);

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    Task<SaleDto?> UpdateSaleAsync(int id, UpdateSaleDto dto);

    /// <summary>
    /// Deletes a sale by ID
    /// </summary>
    Task<bool> DeleteSaleAsync(int id);

    // DASHBOARD METRICS

    /// <summary>
    /// Gets the total count of sales
    /// </summary>
    Task<int> CountAsync();

    /// <summary>
    /// Calculates total revenue from all sales
    /// </summary>
    Task<decimal> GetTotalRevenueAsync();

    /// <summary>
    /// Generates a revenue report grouped by product
    /// </summary>
    Task<IEnumerable<ProductRevenueDto>> GetProductRevenueReportAsync();
}