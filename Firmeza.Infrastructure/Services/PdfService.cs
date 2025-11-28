using Firmeza.Application.Features.Pdf.Interfaces;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmeza.Infrastructure.Services;

/// <summary>
/// Service for generating PDF documents using QuestPDF
/// </summary>
public class PdfService : IPdfService
{
    public PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Generates a PDF document for a sale with client, product and payment details
    /// </summary>
    /// <param name="sale">The sale entity containing all information to be included in the PDF</param>
    /// <returns>Byte array containing the generated PDF document</returns>
    public Task<byte[]> GenerateSalePdfAsync(Sale sale)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Receipt for Sale #{sale.Id}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        col.Spacing(20);

                        // Client Information Section
                        col.Item().Text($"Client: {sale.User?.FullName ?? "Unknown"}").Bold();
                        col.Item().Text($"Document: {sale.User?.DocumentId ?? "-"}");
                        col.Item().Text($"Email: {sale.User?.Email ?? "-"}");
                        col.Item().Text($"Address: {sale.User?.Address ?? "-"}");

                        // Sale Information Section
                        col.Item().Text($"Sale Date: {sale.SaleDate:yyyy-MM-dd HH:mm}");
                        col.Item().Text($"Total Amount: ${sale.TotalAmount:F2}");

                        // Sale Details Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2f);
                                columns.RelativeColumn(1f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Product").Bold();
                                header.Cell().Text("Quantity").Bold();
                                header.Cell().Text("Unit Price").Bold();
                                header.Cell().Text("Total").Bold();
                            });

                            foreach (var detail in sale.SaleDetails)
                            {
                                table.Cell().Text(detail.Product?.Name ?? "Unknown");
                                table.Cell().Text(detail.Quantity.ToString());
                                table.Cell().Text($"${detail.UnitPrice:F2}");
                                table.Cell().Text($"${detail.Quantity * detail.UnitPrice:F2}");
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    /// <summary>
    /// Generates a PDF document with custom report content
    /// </summary>
    /// <param name="reportContent">The text content to include in the report</param>
    /// <returns>Byte array containing the generated PDF document</returns>
    public Task<byte[]> GenerateReportPdfAsync(string reportContent)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Header().Text("Report").FontSize(20).Bold();
                page.Content().Text(reportContent);
                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    /// <summary>
    /// Generates a PDF document listing all products
    /// </summary>
    /// <param name="products">List of products to include in the PDF</param>
    /// <returns>Byte array containing the generated PDF document</returns>
    public Task<byte[]> GenerateProductListPdfAsync(IEnumerable<Firmeza.Application.DTOs.ProductDto> products)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Lista de Productos")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2f); // Name
                            columns.RelativeColumn(1.5f); // Category
                            columns.RelativeColumn(1f); // Price
                            columns.RelativeColumn(1f); // Stock
                            columns.RelativeColumn(3f); // Description
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Nombre");
                            header.Cell().Element(CellStyle).Text("Categoría");
                            header.Cell().Element(CellStyle).Text("Precio");
                            header.Cell().Element(CellStyle).Text("Stock");
                            header.Cell().Element(CellStyle).Text("Descripción");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var product in products)
                        {
                            table.Cell().Element(CellStyle).Text(product.Name);
                            table.Cell().Element(CellStyle).Text(product.Category);
                            table.Cell().Element(CellStyle).Text($"${product.Price:F2}");
                            table.Cell().Element(CellStyle).Text(product.Stock.ToString());
                            table.Cell().Element(CellStyle).Text(product.Description ?? "-");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span($" | Generado el: {DateTime.Now:yyyy-MM-dd HH:mm}");
                    });
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    /// <summary>
    /// Generates a PDF document for the revenue report
    /// </summary>
    public Task<byte[]> GenerateRevenueReportPdfAsync(IEnumerable<Firmeza.Application.DTOs.Sale.ProductRevenueDto> reportData)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Reporte de Ingresos por Producto")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1f); // ID
                            columns.RelativeColumn(3f); // Name
                            columns.RelativeColumn(1.5f); // Quantity
                            columns.RelativeColumn(1.5f); // Unit Price
                            columns.RelativeColumn(1.5f); // Total Revenue
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID");
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).AlignRight().Text("Cant. Vendida");
                            header.Cell().Element(CellStyle).AlignRight().Text("Precio Unit.");
                            header.Cell().Element(CellStyle).AlignRight().Text("Ingresos Totales");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var item in reportData)
                        {
                            table.Cell().Element(CellStyle).Text(item.ProductId.ToString());
                            table.Cell().Element(CellStyle).Text(item.ProductName);
                            table.Cell().Element(CellStyle).AlignRight().Text(item.TotalQuantitySold.ToString());
                            table.Cell().Element(CellStyle).AlignRight().Text($"${item.CurrentUnitPrice:F2}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"${item.TotalRevenue:F2}");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                            }
                        }

                        // Total Row
                        table.Cell().ColumnSpan(4).Element(TotalCellStyle).AlignRight().Text("TOTAL GENERAL:");
                        table.Cell().Element(TotalCellStyle).AlignRight().Text($"${reportData.Sum(x => x.TotalRevenue):F2}");

                        static IContainer TotalCellStyle(IContainer container)
                        {
                            return container.PaddingVertical(5).DefaultTextStyle(x => x.Bold());
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span($" | Generado el: {DateTime.Now:yyyy-MM-dd HH:mm}");
                    });
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }
}

