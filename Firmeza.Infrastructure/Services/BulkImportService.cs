using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Firmeza.Application.DTOs; // CORRECTED: Using the DTO from the Application layer
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;


namespace Firmeza.Infrastructure.Services
{
    public class BulkImportService : IBulkImportService
    {
        private readonly ApplicationDbContext _context;

        public BulkImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BulkImportResultDto> ProcessExcelFileAsync(Stream stream)
        {
            var result = new BulkImportResultDto(); // CORRECTED: Using the DTO
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                result.LogMessages.Add("ERROR: The Excel file is empty or corrupted.");
                return result;
            }

            result.TotalRows = worksheet.Dimension.End.Row - 1;
            var headers = GetHeaders(worksheet);

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                try
                {
                    var clientDocument = GetValue(worksheet, row, headers, "ClientDocument");
                    var clientName = GetValue(worksheet, row, headers, "ClientName");
                    var productName = GetValue(worksheet, row, headers, "ProductName");
                    var saleDateStr = GetValue(worksheet, row, headers, "SaleDate");
                    var quantityStr = GetValue(worksheet, row, headers, "Quantity");
                    var unitPriceStr = GetValue(worksheet, row, headers, "UnitPrice");

                    if (string.IsNullOrWhiteSpace(clientDocument) || string.IsNullOrWhiteSpace(productName) || 
                        !int.TryParse(quantityStr, out int quantity) || !decimal.TryParse(unitPriceStr, out decimal unitPrice))
                    {
                        result.LogMessages.Add($"Row {row}: SKIPPED. Missing required data (ClientDocument, ProductName, Quantity, UnitPrice).");
                        result.FailedRows++;
                        continue;
                    }

                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.Document == clientDocument);
                    if (client == null)
                    {
                        client = new Client { Document = clientDocument, Name = clientName, Email = GetValue(worksheet, row, headers, "ClientEmail", $"{clientDocument}@example.com") };
                        _context.Clients.Add(client);
                        result.LogMessages.Add($"Row {row}: INSERTING new client: {client.Name}");
                        result.SuccessfulInserts++;
                    }
                    else
                    {
                        client.Name = clientName;
                        _context.Clients.Update(client);
                        result.LogMessages.Add($"Row {row}: FOUND existing client: {client.Name}");
                        result.SuccessfulUpdates++;
                    }

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == productName.ToLower());
                    if (product == null)
                    {
                        product = new Product { Name = productName, Price = unitPrice, Stock = 100, Description = "Auto-imported product", Category = "General" };
                        _context.Products.Add(product);
                        result.LogMessages.Add($"Row {row}: INSERTING new product: {product.Name}");
                        result.SuccessfulInserts++;
                    }
                    else
                    {
                        product.Price = unitPrice;
                        _context.Products.Update(product);
                        result.LogMessages.Add($"Row {row}: FOUND existing product: {product.Name}");
                        result.SuccessfulUpdates++;
                    }
                    
                    var sale = new Sale
                    {
                        Client = client,
                        Date = DateTime.TryParse(saleDateStr, out var saleDate) ? saleDate : DateTime.Now,
                        Total = quantity * unitPrice
                    };
                    
                    var saleDetail = new SaleDetail
                    {
                        Sale = sale,
                        Product = product,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    };

                    _context.Sales.Add(sale);
                    _context.SaleDetails.Add(saleDetail);

                    result.ProcessedRows++;
                }
                catch (Exception ex)
                {
                    result.LogMessages.Add($"Row {row}: FAILED. Error: {ex.Message}");
                    result.FailedRows++;
                }
            }

            await _context.SaveChangesAsync();
            result.LogMessages.Add("--- Import Finished. Database has been updated. ---");
            return result;
        }

        private Dictionary<string, int> GetHeaders(ExcelWorksheet worksheet)
        {
            var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var header = worksheet.Cells[1, col].Text;
                if (!string.IsNullOrWhiteSpace(header) && !headers.ContainsKey(header))
                {
                    headers.Add(header, col);
                }
            }
            return headers;
        }

        private string GetValue(ExcelWorksheet ws, int row, Dictionary<string, int> headers, string headerName, string defaultValue = "")
        {
            return headers.TryGetValue(headerName, out int col) ? ws.Cells[row, col].Text : defaultValue;
        }
    }
}
