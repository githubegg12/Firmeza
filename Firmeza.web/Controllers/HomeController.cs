using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Firmeza.web.Models;

namespace Firmeza.web.Controllers;

/// <summary>
/// Main controller for homepage and error handling
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the home page
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.Message = "Firmeza Web is ready!";
        return View();
    }

    /// <summary>
    /// Displays the privacy page
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays error page
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

