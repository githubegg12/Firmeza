using Firmeza.Domain.Entities;
using SaleEntity = Firmeza.Domain.Entities.Sale;

namespace Firmeza.Application.Features.Pdf.Interfaces;

/// <summary>
/// Interfaz para servicio de generación de PDFs
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Genera un PDF a partir de una venta
    /// </summary>
    Task<byte[]> GenerateSalePdfAsync(SaleEntity sale);

    /// <summary>
    /// Genera un PDF a partir de un reporte
    /// </summary>
    Task<byte[]> GenerateReportPdfAsync(string reportContent);

    /// <summary>
    /// Genera un PDF con la lista de productos
    /// </summary>
    Task<byte[]> GenerateProductListPdfAsync(IEnumerable<Firmeza.Application.DTOs.ProductDto> products);

    /// <summary>
    /// Genera un PDF con el reporte de ingresos
    /// </summary>
    Task<byte[]> GenerateRevenueReportPdfAsync(IEnumerable<Firmeza.Application.DTOs.Sale.ProductRevenueDto> reportData);
}

