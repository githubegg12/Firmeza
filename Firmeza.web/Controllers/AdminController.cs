using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers;

[Authorize(Policy = "RequireAdminRole")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

