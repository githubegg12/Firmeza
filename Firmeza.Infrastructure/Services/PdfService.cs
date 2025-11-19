using Firmeza.Application.Features.Pdf;
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
                        col.Item().Text($"Client: {sale.Client.Name}").Bold();
                        col.Item().Text($"Document: {sale.Client.Document}");
                        col.Item().Text($"Email: {sale.Client.Email}");
                        col.Item().Text($"Phone: {sale.Client.Phone}");

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
}



