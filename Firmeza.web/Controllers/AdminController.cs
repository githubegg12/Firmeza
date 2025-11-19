using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers;

/// <summary>
/// Controller for admin dashboard and management
/// </summary>
[Authorize(Policy = "RequireAdminRole")]
public class AdminController : Controller
{
    /// <summary>
    /// Displays the admin index page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displays the admin dashboard
    /// </summary>
    public IActionResult Dashboard()
    {
        return View();
    }
}

