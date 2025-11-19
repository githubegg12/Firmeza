using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.BulkImport;

/// <summary>
/// Interfaz para servicio de importación masiva
/// </summary>
public interface IBulkImportService
{
    /// <summary>
    /// Procesa un archivo Excel y retorna el resultado de la importación
    /// </summary>
    Task<BulkImportResultDto> ProcessExcelFileAsync(Stream fileStream);
}

