using Firmeza.Domain.Entities;

namespace Firmeza.Application.Features.Pdf;

/// <summary>
/// Interfaz para servicio de generación de PDFs
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Genera un PDF a partir de una venta
    /// </summary>
    Task<byte[]> GenerateSalePdfAsync(Sale sale);

    /// <summary>
    /// Genera un PDF a partir de un reporte
    /// </summary>
    Task<byte[]> GenerateReportPdfAsync(string reportContent);
}

