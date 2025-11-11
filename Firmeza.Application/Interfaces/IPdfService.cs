using Firmeza.Domain.Entities;
using System.Threading.Tasks;

namespace Firmeza.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that generates PDF documents.
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Generates a PDF receipt for a given sale.
        /// </summary>
        /// <param name="sale">The sale entity, which must include its related Client and SaleDetails.</param>
        /// <returns>The web-accessible path to the generated PDF file (e.g., "/receipts/sale_123.pdf").</returns>
        Task<string> GenerateSaleReceiptAsync(Sale sale);
    }
}
