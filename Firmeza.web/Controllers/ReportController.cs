using Firmeza.Application.Features.Pdf.Interfaces;
using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Sale.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers;

[Authorize(Roles = "Administrador")]
public class ReportController : Controller
{
    private readonly ISaleService _saleService;
    private readonly IPdfService _pdfService;

    public ReportController(ISaleService saleService, IPdfService pdfService)
    {
        _saleService = saleService;
        _pdfService = pdfService;
    }

    /// <summary>
    /// Displays the product revenue report
    /// </summary>
    public async Task<IActionResult> RevenueReport()
    {
        var report = await _saleService.GetProductRevenueReportAsync();
        return View(report);
    }

    /// <summary>
    /// Exports the revenue report to PDF
    /// </summary>
    public async Task<IActionResult> ExportRevenueReportPdf()
    {
        var report = await _saleService.GetProductRevenueReportAsync();
        var pdfBytes = await _pdfService.GenerateRevenueReportPdfAsync(report);
        return File(pdfBytes, "application/pdf", $"ReporteIngresos_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
    }
}
