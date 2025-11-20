using Firmeza.Application.Features.Pdf;
using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace Firmeza.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;

        public SaleController(ApplicationDbContext context, IPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales.Include(s => s.Client).ToListAsync();
            return View(sales);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
                return NotFound();

            return View(sale);
        }

        // GET: Sales/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Clients"] = await _context.Clients.ToListAsync();
            ViewData["Products"] = await _context.Products.ToListAsync();
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int clientId, int productId, int quantity)
        {
            var client = await _context.Clients.FindAsync(clientId);
            var product = await _context.Products.FindAsync(productId);

            if (client == null || product == null || quantity <= 0)
            {
                return BadRequest("Invalid client, product, or quantity.");
            }

            var sale = new Sale
            {
                Client = client,
                SaleDate = DateTime.UtcNow,
                TotalAmount = product.Price * quantity,
                SaleDetails = new[]
                {
                    new SaleDetail { Product = product, Quantity = quantity, UnitPrice = product.Price }
                }
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return NotFound();

            ViewData["Clients"] = await _context.Clients.ToListAsync();
            ViewData["Products"] = await _context.Products.ToListAsync();
            return View(sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int clientId, int productId, int quantity)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return NotFound();

            var client = await _context.Clients.FindAsync(clientId);
            var product = await _context.Products.FindAsync(productId);

            if (client == null || product == null || quantity <= 0)
            {
                return BadRequest("Invalid client, product, or quantity.");
            }

            // Update Sale
            sale.Client = client;
            sale.TotalAmount = product.Price * quantity;
            
            // Update Detail (Assuming single product per sale for now)
            var detail = sale.SaleDetails.FirstOrDefault();
            if (detail != null)
            {
                detail.Product = product;
                detail.Quantity = quantity;
                detail.UnitPrice = product.Price;
            }
            else
            {
                // Should not happen based on Create logic, but handle just in case
                sale.SaleDetails = new[]
                {
                    new SaleDetail { Product = product, Quantity = quantity, UnitPrice = product.Price }
                };
            }

            _context.Update(sale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
                return NotFound();

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Export Sale to PDF
        [HttpGet]
        public async Task<IActionResult> ExportToPdf(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return NotFound();

            var pdfBytes = await _pdfService.GenerateSalePdfAsync(sale);
            return File(pdfBytes, "application/pdf", $"Sale_{sale.Id}.pdf");
        }
    }
}

