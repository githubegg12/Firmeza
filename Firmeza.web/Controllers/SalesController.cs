using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Firmeza.web.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;

        public SalesController(ApplicationDbContext context, IPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // A simple GET action to list sales
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales.Include(s => s.Client).ToListAsync();
            return View(sales);
        }

        // This is a simplified Create method for demonstration.
        // In a real app, this would take a ViewModel from a form.
        [HttpPost]
        public async Task<IActionResult> CreateSale(int clientId, int productId, int quantity)
        {
            var client = await _context.Clients.FindAsync(clientId);
            var product = await _context.Products.FindAsync(productId);

            if (client == null || product == null || quantity <= 0)
            {
                return BadRequest("Invalid client, product, or quantity.");
            }

            // 1. Create the Sale and SaleDetail
            var sale = new Sale
            {
                Client = client,
                SaleDate = System.DateTime.Now,
                TotalAmount = product.Price * quantity,
                SaleDetails = new[]
                {
                    new SaleDetail { Product = product, Quantity = quantity, UnitPrice = product.Price }
                }
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync(); // First save to get a Sale.Id

            // 2. Generate the PDF receipt
            // We need to reload the sale with its navigation properties for the PDF service
            var saleForPdf = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == sale.Id);

            if (saleForPdf != null)
            {
                var receiptUrl = await _pdfService.GenerateSaleReceiptAsync(saleForPdf);

                // 3. Update the sale with the receipt URL
                sale.ReceiptUrl = receiptUrl;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync(); // Second save to store the URL
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
