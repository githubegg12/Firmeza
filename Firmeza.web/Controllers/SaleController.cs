using Firmeza.Application.Features.Pdf.Interfaces;
using Firmeza.Application.Features.Email.Interfaces;
using Firmeza.Application.DTOs.Sale;
using Firmeza.Application.Features.Sale.Interfaces;
using Firmeza.Domain.Entities;

using Firmeza.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
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
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SaleController(ApplicationDbContext context, IPdfService pdfService, IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _pdfService = pdfService;
            _emailService = emailService;
            _userManager = userManager;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales.Include(s => s.User).ToListAsync();
            return View(sales);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.User)
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
            var users = await _userManager.GetUsersInRoleAsync("Cliente");
            ViewData["Users"] = users.ToList();
            ViewData["Products"] = await _context.Products.ToListAsync();
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string userId, int productId, int quantity)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var product = await _context.Products.FindAsync(productId);

            if (user == null || product == null || quantity <= 0)
            {
                TempData["Error"] = "Usuario, producto o cantidad inválidos.";
                var users = await _userManager.GetUsersInRoleAsync("Cliente");
                ViewData["Users"] = users.ToList();
                ViewData["Products"] = await _context.Products.ToListAsync();
                return View();
            }

            // ✅ VALIDACIÓN DE STOCK
            if (product.Stock < quantity)
            {
                TempData["Error"] = $"Stock insuficiente. Disponible: {product.Stock}, Solicitado: {quantity}";
                var users = await _userManager.GetUsersInRoleAsync("Cliente");
                ViewData["Users"] = users.ToList();
                ViewData["Products"] = await _context.Products.ToListAsync();
                return View();
            }

            // ✅ DESCONTAR STOCK
            product.Stock -= quantity;

            var sale = new Sale
            {
                UserId = userId,
                User = user,
                SaleDate = DateTime.UtcNow,
                TotalAmount = product.Price * quantity,
                SaleDetails = new[]
                {
                    new SaleDetail { Product = product, Quantity = quantity, UnitPrice = product.Price }
                }
            };

            _context.Sales.Add(sale);
            _context.Products.Update(product); // Actualizar producto con nuevo stock
            await _context.SaveChangesAsync();

            // Send purchase confirmation email
            try
            {
                var orderDetails = $@"
                    <p><strong>Cliente:</strong> {user.FullName}</p>
                    <p><strong>Producto:</strong> {product.Name}</p>
                    <p><strong>Cantidad:</strong> {quantity}</p>
                    <p><strong>Precio Unitario:</strong> ${product.Price:N2}</p>
                    <p><strong>Total:</strong> ${sale.TotalAmount:N2}</p>
                    <p><strong>Fecha:</strong> {sale.SaleDate:dd/MM/yyyy HH:mm}</p>
                ";
                await _emailService.SendPurchaseConfirmationAsync(user.Email!, orderDetails);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the sale
                Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
            }

            TempData["Success"] = $"Venta creada exitosamente. Stock restante: {product.Stock}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return NotFound();

            var users = await _userManager.GetUsersInRoleAsync("Cliente");
            ViewData["Users"] = users.ToList();
            ViewData["Products"] = await _context.Products.ToListAsync();
            return View(sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string userId, int productId, int quantity)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            var newProduct = await _context.Products.FindAsync(productId);

            if (user == null || newProduct == null || quantity <= 0)
            {
                TempData["Error"] = "Usuario, producto o cantidad inválidos.";
                var users = await _userManager.GetUsersInRoleAsync("Cliente");
                ViewData["Users"] = users.ToList();
                ViewData["Products"] = await _context.Products.ToListAsync();
                return View(sale);
            }

            // Obtener el detalle anterior
            var oldDetail = sale.SaleDetails.FirstOrDefault();
            
            if (oldDetail != null)
            {
                var oldProduct = oldDetail.Product;
                var oldQuantity = oldDetail.Quantity;

                // ✅ RESTAURAR STOCK DEL PRODUCTO ANTERIOR
                oldProduct.Stock += oldQuantity;
                _context.Products.Update(oldProduct);

                // ✅ VALIDAR STOCK DEL NUEVO PRODUCTO
                if (newProduct.Stock < quantity)
                {
                    TempData["Error"] = $"Stock insuficiente para {newProduct.Name}. Disponible: {newProduct.Stock}, Solicitado: {quantity}";
                    var users = await _userManager.GetUsersInRoleAsync("Cliente");
                    ViewData["Users"] = users.ToList();
                    ViewData["Products"] = await _context.Products.ToListAsync();
                    
                    // Revertir el cambio de stock del producto anterior
                    oldProduct.Stock -= oldQuantity;
                    return View(sale);
                }

                // ✅ DESCONTAR STOCK DEL NUEVO PRODUCTO
                newProduct.Stock -= quantity;
                _context.Products.Update(newProduct);

                // Actualizar el detalle
                oldDetail.Product = newProduct;
                oldDetail.Quantity = quantity;
                oldDetail.UnitPrice = newProduct.Price;
            }
            else
            {
                // Caso raro: no hay detalle previo
                if (newProduct.Stock < quantity)
                {
                    TempData["Error"] = $"Stock insuficiente. Disponible: {newProduct.Stock}, Solicitado: {quantity}";
                    var users = await _userManager.GetUsersInRoleAsync("Cliente");
                    ViewData["Users"] = users.ToList();
                    ViewData["Products"] = await _context.Products.ToListAsync();
                    return View(sale);
                }

                newProduct.Stock -= quantity;
                _context.Products.Update(newProduct);

                sale.SaleDetails = new[]
                {
                    new SaleDetail { Product = newProduct, Quantity = quantity, UnitPrice = newProduct.Price }
                };
            }

            // Update Sale
            sale.UserId = userId;
            sale.User = user;
            sale.TotalAmount = newProduct.Price * quantity;

            _context.Update(sale);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Venta actualizada exitosamente. Stock de {newProduct.Name}: {newProduct.Stock}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.User)
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
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale != null)
            {
                // ✅ RESTAURAR STOCK DE LOS PRODUCTOS
                foreach (var detail in sale.SaleDetails)
                {
                    var product = detail.Product;
                    product.Stock += detail.Quantity;
                    _context.Products.Update(product);
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Venta eliminada y stock restaurado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Export Sale to PDF
        [HttpGet]
        public async Task<IActionResult> ExportToPdf(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.User)
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

