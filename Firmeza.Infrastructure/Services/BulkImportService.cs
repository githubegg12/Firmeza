using Firmeza.Application.Features.BulkImport;
using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Firmeza.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Firmeza.Infrastructure.Services;

/// <summary>
/// Service for bulk importing products from Excel files
/// </summary>
public class BulkImportService : IBulkImportService
{
    private readonly ApplicationDbContext _context;

    public BulkImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BulkImportResultDto> ProcessExcelFileAsync(Stream stream)
    {
        var result = new BulkImportResultDto();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        try
        {
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
            {
                result.LogMessages.Add("ERROR: Excel file is empty or corrupted.");
                return result;
            }

            result.TotalRows = worksheet.Dimension?.End.Row - 1 ?? 0;
            if (result.TotalRows == 0)
            {
                result.LogMessages.Add("ERROR: No data found in file.");
                return result;
            }

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
                        result.LogMessages.Add($"Row {row}: SKIPPED. Missing required data.");
                        result.FailedRows++;
                        continue;
                    }

                    // Process Client
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.Document == clientDocument);
                    if (client == null)
                    {
                        client = new Client
                        {
                            Document = clientDocument,
                            Name = clientName ?? "Unknown Client",
                            Email = GetValue(worksheet, row, headers, "ClientEmail", $"{clientDocument}@example.com"),
                            Phone = GetValue(worksheet, row, headers, "ClientPhone", ""),
                            Address = GetValue(worksheet, row, headers, "ClientAddress", "")
                        };
                        _context.Clients.Add(client);
                        result.LogMessages.Add($"Row {row}: INSERTING new client: {client.Name}");
                        result.SuccessfulInserts++;
                    }
                    else
                    {
                        result.LogMessages.Add($"Row {row}: Existing client found: {client.Name}");
                        result.SuccessfulUpdates++;
                    }

                    // Process Product
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == productName.ToLower());
                    if (product == null)
                    {
                        product = new Product
                        {
                            Name = productName,
                            Price = unitPrice,
                            Stock = 100,
                            Description = "Auto-imported product",
                            Category = GetValue(worksheet, row, headers, "ProductCategory", "General")
                        };
                        _context.Products.Add(product);
                        result.LogMessages.Add($"Row {row}: INSERTING new product: {product.Name}");
                        result.SuccessfulInserts++;
                    }
                    else
                    {
                        result.LogMessages.Add($"Row {row}: Existing product found: {product.Name}");
                    }

                    // Create Sale
                    var sale = new Sale
                    {
                        Client = client,
                        SaleDate = DateTime.TryParse(saleDateStr, out var saleDate) ? saleDate : DateTime.Now,
                        TotalAmount = quantity * unitPrice
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
                    
                    result.LogMessages.Add($"Row {row}: Sale created for {client.Name}");
                    result.SuccessfulInserts++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.LogMessages.Add($"Row {row}: ERROR - {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            result.LogMessages.Add($"Import completed: {result.SuccessfulInserts} inserted, {result.FailedRows} skipped.");
        }
        catch (Exception ex)
        {
            result.LogMessages.Add($"General error: {ex.Message}");
        }

        return result;
    }

    private static Dictionary<string, int> GetHeaders(ExcelWorksheet worksheet)
    {
        var headers = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var headerValue = worksheet.Cells[1, col].Value?.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                headers[headerValue] = col;
            }
        }
        return headers;
    }

    private static string GetValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, string headerName, string? defaultValue = null)
    {
        if (headers.TryGetValue(headerName, out int col))
        {
            return worksheet.Cells[row, col].Value?.ToString() ?? defaultValue ?? string.Empty;
        }
        return defaultValue ?? string.Empty;
    }
}

