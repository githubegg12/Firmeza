using Firmeza.Application.Interfaces;
using Firmeza.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers;

[Authorize(Policy = "RequireAdminRole")]
public class DashboardController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly IClientService _clientService;
    private readonly IProductService _productService;
    private readonly ISalesService _salesService;

    public DashboardController(
        IClientService clientService,
        IProductService productService,
        ISalesService salesService)
    {
        _clientService = clientService;
        _productService = productService;
        _salesService = salesService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            TotalClients = await _clientService.CountAsync(),
            TotalProducts = await _productService.CountAsync(),
            TotalSales = await _salesService.CountAsync(),
            TotalRevenue = await _salesService.GetTotalRevenueAsync(),
            LastUpdated = DateTime.Now
        };

        return View(model);
    }
}
