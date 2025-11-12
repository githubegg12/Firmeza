using Firmeza.Infrastructure.Data;
using Firmeza.Domain.Entities;
using Firmeza.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Firmeza.web.Controllers
{
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
                var client = new Client
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
            if (id == null) return NotFound();
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

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
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var client = await _context.Clients.FindAsync(id);
                    if (client == null) return NotFound();

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
                    if (!_context.Clients.Any(e => e.Id == viewModel.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var client = await _context.Clients.FirstOrDefaultAsync(m => m.Id == id);
            if (client == null) return NotFound();
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

        #region Export Actions

        // GET: Clients/ExportToExcel
        public async Task<IActionResult> ExportToExcel()
        {
            var clients = await _context.Clients.ToListAsync();
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Clients");

            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Document";
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Phone";

            for (int i = 0; i < clients.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = clients[i].Id;
                worksheet.Cells[i + 2, 2].Value = clients[i].Name;
                worksheet.Cells[i + 2, 3].Value = clients[i].Document;
                worksheet.Cells[i + 2, 4].Value = clients[i].Email;
                worksheet.Cells[i + 2, 5].Value = clients[i].Phone;
            }

            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;
            
            var fileName = $"Clients_{System.DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: Clients/ExportToPdf
        public async Task<IActionResult> ExportToPdf()
        {
            var clients = await _context.Clients.ToListAsync();
            
            QuestPDF.Settings.License = LicenseType.Community;
            var pdfData = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Clients List").SemiBold().FontSize(24);
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("ID");
                            header.Cell().Text("Name");
                            header.Cell().Text("Document");
                            header.Cell().Text("Email");
                        });

                        foreach (var client in clients)
                        {
                            table.Cell().Text(client.Id.ToString());
                            table.Cell().Text(client.Name);
                            table.Cell().Text(client.Document);
                            table.Cell().Text(client.Email);
                        }
                    });
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf();

            var fileName = $"Clients_{System.DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(pdfData, "application/pdf", fileName);
        }

        #endregion
    }
}
