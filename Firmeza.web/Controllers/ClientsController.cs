using Firmeza.Infrastructure.Data;
using Firmeza.Domain.Entities;
using Firmeza.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.web.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        // Handles listing clients and searching by Name or Document
        public async Task<IActionResult> Index(string searchString)
        {
            var clientsQuery = _context.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                clientsQuery = clientsQuery.Where(c => 
                    c.Name.Contains(searchString) || 
                    c.Document.Contains(searchString)
                );
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
    }
}
