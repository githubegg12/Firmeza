using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Product.Interfaces;
using Firmeza.Application.Features.Sale.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers;

/// <summary>
/// Admin dashboard controller using clean architecture with services
/// </summary>
[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProductMetricsService _productMetricsService;
    private readonly ISaleService _saleService;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        IProductMetricsService productMetricsService,
        ISaleService saleService)
    {
        _userManager = userManager;
        _productMetricsService = productMetricsService;
        _saleService = saleService;
    }

    /// <summary>
    /// Displays the admin dashboard with real-time metrics
    /// Uses efficient COUNT queries instead of loading all data
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var clients = await _userManager.GetUsersInRoleAsync("Cliente");
        
        var viewModel = new DashboardViewModel
        {
            TotalClients = clients.Count,
            TotalProducts = await _productMetricsService.CountAsync(),
            TotalSales = await _saleService.CountAsync(),
            TotalRevenue = await _saleService.GetTotalRevenueAsync(),
            LastUpdated = DateTime.Now
        };

        return View(viewModel);
    }

    /// <summary>
    /// Displays the admin dashboard (alternative route)
    /// </summary>
    public async Task<IActionResult> Dashboard()
    {
        return await Index();
    }


}
