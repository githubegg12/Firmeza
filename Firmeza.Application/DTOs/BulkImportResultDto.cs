using System.Collections.Generic;

namespace Firmeza.Application.DTOs
{
    /// <summary>
    /// Represents the result of a bulk import operation.
    /// This is a Data Transfer Object (DTO) used to pass data from the Application layer outwards.
    /// </summary>
    public class BulkImportResultDto
    {
        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public int SuccessfulInserts { get; set; }
        public int SuccessfulUpdates { get; set; }
        public int FailedRows { get; set; }
        public List<string> LogMessages { get; set; } = new List<string>();
    }
}
