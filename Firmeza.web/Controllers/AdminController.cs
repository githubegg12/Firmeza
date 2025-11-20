using Firmeza.Domain.Interfaces;
using Firmeza.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.web.Controllers;

/// <summary>
/// Controller for admin dashboard and management
/// </summary>
[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ISaleRepository _saleRepository;

    public AdminController(
        IProductRepository productRepository,
        IClientRepository clientRepository,
        ISaleRepository saleRepository)
    {
        _productRepository = productRepository;
        _clientRepository = clientRepository;
        _saleRepository = saleRepository;
    }

    /// <summary>
    /// Displays the admin dashboard with real-time metrics
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var clients = await _clientRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        var sales = await _saleRepository.GetAllAsync();

        var model = new DashboardViewModel
        {
            TotalClients = clients.Count(),
            TotalProducts = products.Count(),
            TotalSales = sales.Count(),
            TotalRevenue = sales.Sum(s => s.TotalAmount),
            LastUpdated = DateTime.Now
        };

        return View(model);
    }

    /// <summary>
    /// Displays the admin dashboard (alternative route)
    /// </summary>
    public async Task<IActionResult> Dashboard()
    {
        return await Index();
    }
}

