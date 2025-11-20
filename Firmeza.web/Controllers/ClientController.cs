using Firmeza.Infrastructure.Data;
using Firmeza.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using Microsoft.AspNetCore.Authorization;

namespace Firmeza.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string searchString)
        {
            var clientsQuery = _context.Clients.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                clientsQuery = clientsQuery.Where(c => c.Name.Contains(searchString) || c.Document.Contains(searchString));
            }
            var clients = await clientsQuery.ToListAsync();
            ViewData["CurrentFilter"] = searchString;
            return View(clients);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var client = new Domain.Entities.Client
                {
                    Name = viewModel.Name,
                    Document = viewModel.Document,
                    Email = viewModel.Email,
                    Phone = viewModel.Phone,
                    Address = viewModel.Address
                };

                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            var viewModel = new ClientViewModel
            {
                Id = client.Id,
                Name = client.Name,
                Document = client.Document,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };

            return View(viewModel);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var client = await _context.Clients.FindAsync(id);
                    if (client == null)
                        return NotFound();

                    client.Name = viewModel.Name;
                    client.Document = viewModel.Document;
                    client.Email = viewModel.Email;
                    client.Phone = viewModel.Phone;
                    client.Address = viewModel.Address;

                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(viewModel.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients.FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
                return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        // Export to Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var clients = await _context.Clients.ToListAsync();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Clients");
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "ClientName";
                    worksheet.Cells[1, 3].Value = "ClientDocument";
                    worksheet.Cells[1, 4].Value = "ClientEmail";
                    worksheet.Cells[1, 5].Value = "ClientPhone";
                    worksheet.Cells[1, 6].Value = "ClientAddress";

                    int row = 2;
                    foreach (var client in clients)
                    {
                        worksheet.Cells[row, 1].Value = client.Id;
                        worksheet.Cells[row, 2].Value = client.Name;
                        worksheet.Cells[row, 3].Value = client.Document;
                        worksheet.Cells[row, 4].Value = client.Email;
                        worksheet.Cells[row, 5].Value = client.Phone;
                        worksheet.Cells[row, 6].Value = client.Address;
                        row++;
                    }

                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clients.xlsx");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting Excel: {ex}");
                return BadRequest($"Error exporting Excel: {ex.Message}");
            }
        }

        // Export to PDF
        [HttpGet]
        public async Task<IActionResult> ExportToPdf()
        {
            try
            {
                var clients = await _context.Clients.ToListAsync();

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.Header().Text("Clients Report").FontSize(20).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("ID").Bold();
                                header.Cell().Text("Name").Bold();
                                header.Cell().Text("Email").Bold();
                                header.Cell().Text("Phone").Bold();
                                header.Cell().Text("Document").Bold();
                            });

                            foreach (var client in clients)
                            {
                                table.Cell().Text(client.Id.ToString());
                                table.Cell().Text(client.Name);
                                table.Cell().Text(client.Email);
                                table.Cell().Text(client.Phone);
                                table.Cell().Text(client.Document);
                            }
                        });
                    });
                });

                var pdf = document.GeneratePdf();
                return File(pdf, "application/pdf", "Clients.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting PDF: {ex}");
                return BadRequest($"Error exporting PDF: {ex.Message}");
            }
        }
    }
}

