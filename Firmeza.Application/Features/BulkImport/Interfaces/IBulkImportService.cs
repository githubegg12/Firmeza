using Firmeza.Application.DTOs;

namespace Firmeza.Application.Features.BulkImport;

/// <summary>
/// Interface for bulk import service
/// </summary>
public interface IBulkImportService
{
    /// <summary>
    /// Processes an Excel file and returns the import result
    /// </summary>
    Task<BulkImportResultDto> ProcessExcelFileAsync(Stream fileStream);
}

