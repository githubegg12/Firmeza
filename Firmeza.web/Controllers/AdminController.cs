using Firmeza.Infrastructure.Data;
using Firmeza.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.web.Controllers;

/// <summary>
/// Controller for the administrative dashboard area.
/// Access is restricted to users with the "Administrador" role.
/// </summary>
[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Index or /Admin
    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalClients = await _context.Clients.CountAsync(),
            TotalSales = await _context.Sales.CountAsync()
        };
        
        return View(viewModel);
    }
}