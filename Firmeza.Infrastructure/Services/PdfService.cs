using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Microsoft.AspNetCore.Hosting; // CORRECTED: Added the missing using for IWebHostEnvironment
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Firmeza.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PdfService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<string> GenerateSaleReceiptAsync(Sale sale)
        {
            // Define file path and URL
            var fileName = $"receipt_sale_{sale.Id}_{Guid.NewGuid().ToString().Substring(0, 8)}.pdf";
            var directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, "receipts");
            var filePath = Path.Combine(directoryPath, fileName);
            var fileUrl = $"/receipts/{fileName}";

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Generate the PDF document
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

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text("Client Information").SemiBold();
                                    column.Item().Text(sale.Client.Name);
                                    column.Item().Text(sale.Client.Document);
                                    column.Item().Text(sale.Client.Email);
                                });

                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text("Sale Information").SemiBold();
                                    column.Item().Text($"Sale ID: {sale.Id}");
                                    column.Item().Text($"Date: {sale.SaleDate:yyyy-MM-dd}");
                                });
                            });

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Product Name
                                    columns.RelativeColumn();  // Quantity
                                    columns.RelativeColumn();  // Unit Price
                                    columns.RelativeColumn();  // Total
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Product").Bold();
                                    header.Cell().AlignRight().Text("Quantity").Bold();
                                    header.Cell().AlignRight().Text("Unit Price").Bold();
                                    header.Cell().AlignRight().Text("Total").Bold();
                                });

                                foreach (var item in sale.SaleDetails)
                                {
                                    table.Cell().Text(item.Product.Name);
                                    table.Cell().AlignRight().Text(item.Quantity.ToString());
                                    table.Cell().AlignRight().Text($"{item.UnitPrice:C}");
                                    table.Cell().AlignRight().Text($"{(item.Quantity * item.UnitPrice):C}");
                                }
                            });

                            var subtotal = sale.SaleDetails.Sum(d => d.Quantity * d.UnitPrice);
                            var iva = subtotal * 0.19m; // Assuming 19% IVA
                            var total = subtotal + iva;

                            col.Item().AlignRight().Text($"Subtotal: {subtotal:C}").SemiBold();
                            col.Item().AlignRight().Text($"IVA (19%): {iva:C}").SemiBold();
                            col.Item().AlignRight().Text($"Total: {total:C}").Bold().FontSize(14);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            // Generate the PDF file on disk
            await Task.Run(() => document.GeneratePdf(filePath));

            return fileUrl;
        }
    }
}
