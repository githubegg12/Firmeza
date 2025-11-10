using System.IO;
using System.Threading.Tasks;
using Firmeza.Application.DTOs; // CORRECTED: Now references its own layer.

namespace Firmeza.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for the bulk import service.
    /// </summary>
    public interface IBulkImportService
    {
        /// <summary>
        /// Processes a denormalized Excel file stream, normalizes the data,
        /// validates it, and persists it to the database.
        /// </summary>
        /// <param name="stream">The stream of the .xlsx file.</param>
        /// <returns>A DTO containing the results of the import operation.</returns>
        Task<BulkImportResultDto> ProcessExcelFileAsync(Stream stream);
    }
}
