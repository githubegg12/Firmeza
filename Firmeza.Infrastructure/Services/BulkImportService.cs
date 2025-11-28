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
                bool rowProcessed = false;
                try
                {
                    // Track processed items in this batch to prevent in-file duplicates
                    var processedProductNames = new HashSet<string>();

                    // Extract potential data
                    var productName = GetValue(worksheet, row, headers, "ProductName");
                    var quantityStr = GetValue(worksheet, row, headers, "Quantity");
                    var unitPriceStr = GetValue(worksheet, row, headers, "UnitPrice");

                    Product? product = null;

                    // --- 1. PROCESS PRODUCT ---
                    if (!string.IsNullOrWhiteSpace(productName))
                    {
                        // Check for duplicate in current file
                        if (processedProductNames.Contains(productName.ToLower()))
                        {
                            result.LogMessages.Add($"Row {row}: [PRODUCT] SKIPPED. Duplicate product '{productName}' found in file.");
                             // Try to find in local context for sale
                            product = _context.Products.Local.FirstOrDefault(p => p.Name.ToLower() == productName.ToLower());
                        }
                        else
                        {
                            product = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == productName.ToLower());
                            
                            decimal.TryParse(unitPriceStr, out decimal price);
                            
                            if (product == null)
                            {
                                product = new Product
                                {
                                    Name = productName,
                                    Price = price > 0 ? price : 0,
                                    Stock = int.TryParse(GetValue(worksheet, row, headers, "Stock"), out int stock) ? stock : 100,
                                    Description = GetValue(worksheet, row, headers, "Description", "Auto-imported product"),
                                    Category = GetValue(worksheet, row, headers, "ProductCategory", "General")
                                };
                                _context.Products.Add(product);
                                processedProductNames.Add(productName.ToLower());
                                result.LogMessages.Add($"Row {row}: [PRODUCT] Creating new product '{product.Name}'");
                                result.SuccessfulInserts++;
                            }
                            else
                            {
                                // STRICT VALIDATION: Do not update, just skip.
                                result.LogMessages.Add($"Row {row}: [PRODUCT] SKIPPED. Product '{product.Name}' already exists in DB.");
                                processedProductNames.Add(productName.ToLower());
                            }
                        }
                        rowProcessed = true;
                    }

                    // --- CLIENT AND SALE IMPORT TEMPORARILY DISABLED DUE TO ARCHITECTURE CHANGE ---
                    /*
                    // Client and Sale logic removed as Client entity is merged into ApplicationUser.
                    // TODO: Implement User import logic if needed.
                    */

                    if (!rowProcessed)
                    {
                        result.LogMessages.Add($"Row {row}: SKIPPED. No valid Product Name found.");
                        result.FailedRows++;
                    }
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.LogMessages.Add($"Row {row}: ERROR - {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            result.LogMessages.Add($"Import completed. Processed {result.TotalRows} rows.");
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

