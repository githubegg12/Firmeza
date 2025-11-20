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
                    var processedClientDocuments = new HashSet<string>();
                    var processedProductNames = new HashSet<string>();

                    // Extract potential data
                    var clientDocument = GetValue(worksheet, row, headers, "ClientDocument");
                    var clientName = GetValue(worksheet, row, headers, "ClientName");
                    var productName = GetValue(worksheet, row, headers, "ProductName");
                    var saleDateStr = GetValue(worksheet, row, headers, "SaleDate");
                    var quantityStr = GetValue(worksheet, row, headers, "Quantity");
                    var unitPriceStr = GetValue(worksheet, row, headers, "UnitPrice");

                    Client? client = null;
                    Product? product = null;

                    // --- 1. PROCESS CLIENT ---
                    if (!string.IsNullOrWhiteSpace(clientDocument))
                    {
                        // Check for duplicate in current file
                        if (processedClientDocuments.Contains(clientDocument))
                        {
                             result.LogMessages.Add($"Row {row}: [CLIENT] SKIPPED. Duplicate document '{clientDocument}' found in file.");
                             // We still try to fetch it to process the sale if possible, or just skip?
                             // If it's a duplicate in file, we might want to use the PREVIOUSLY loaded client for the sale?
                             // But for now, let's assume we skip the creation/update.
                             // To allow sale processing, we should try to find it in context (it might be added in previous iteration).
                             client = _context.Clients.Local.FirstOrDefault(c => c.Document == clientDocument);
                        }
                        else
                        {
                            client = await _context.Clients.FirstOrDefaultAsync(c => c.Document == clientDocument);
                            if (client == null)
                            {
                                client = new Client
                                {
                                    Document = clientDocument,
                                    Name = !string.IsNullOrWhiteSpace(clientName) ? clientName : "Unknown Client",
                                    Email = GetValue(worksheet, row, headers, "ClientEmail", $"{clientDocument}@example.com"),
                                    Phone = GetValue(worksheet, row, headers, "ClientPhone", ""),
                                    Address = GetValue(worksheet, row, headers, "ClientAddress", "")
                                };
                                _context.Clients.Add(client);
                                processedClientDocuments.Add(clientDocument);
                                result.LogMessages.Add($"Row {row}: [CLIENT] Creating new client '{client.Name}'");
                                result.SuccessfulInserts++;
                            }
                            else
                            {
                                // STRICT VALIDATION: Do not update, just skip.
                                result.LogMessages.Add($"Row {row}: [CLIENT] SKIPPED. Client '{client.Name}' ({client.Document}) already exists in DB.");
                                processedClientDocuments.Add(clientDocument); // Mark as seen so we don't log "Duplicate in file" for next rows
                            }
                        }
                        rowProcessed = true;
                    }

                    // --- 2. PROCESS PRODUCT ---
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

                    // --- 3. PROCESS SALE (Requires Client + Product + Quantity + Price) ---
                    if (client != null && product != null && 
                        int.TryParse(quantityStr, out int quantity) && quantity > 0 &&
                        decimal.TryParse(unitPriceStr, out decimal unitPrice) && unitPrice > 0)
                    {
                        // Need to ensure client and product are tracked/saved before using them in Sale?
                        // EF Core handles this if they are added to context.
                        
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
                        
                        result.LogMessages.Add($"Row {row}: [SALE] Registered sale for '{client.Name}' - Item: '{product.Name}'");
                        result.SuccessfulInserts++;
                        rowProcessed = true;
                    }
                    else if (client != null && product != null)
                    {
                         // Had client and product but missing sale details
                         result.LogMessages.Add($"Row {row}: [INFO] Client and Product present, but missing Quantity/Price for Sale.");
                    }

                    if (!rowProcessed)
                    {
                        result.LogMessages.Add($"Row {row}: SKIPPED. No valid Client Document or Product Name found.");
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

