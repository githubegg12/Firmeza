using Firmeza.Domain.Entities;
using SaleEntity = Firmeza.Domain.Entities.Sale;

namespace Firmeza.Application.Features.Pdf.Interfaces;

/// <summary>
/// Interface for PDF generation service
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Generates a PDF for a specific sale
    /// </summary>
    Task<byte[]> GenerateSalePdfAsync(SaleEntity sale);

    /// <summary>
    /// Generates a PDF from a report string content
    /// </summary>
    Task<byte[]> GenerateReportPdfAsync(string reportContent);

    /// <summary>
    /// Generates a PDF containing a list of products
    /// </summary>
    Task<byte[]> GenerateProductListPdfAsync(IEnumerable<Firmeza.Application.DTOs.ProductDto> products);

    /// <summary>
    /// Generates a PDF containing a revenue report
    /// </summary>
    Task<byte[]> GenerateRevenueReportPdfAsync(IEnumerable<Firmeza.Application.DTOs.Sale.ProductRevenueDto> reportData);
}

