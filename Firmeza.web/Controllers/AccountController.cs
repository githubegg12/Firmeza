using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Email.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(IAuthService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    // GET: Login
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"),
        });
    }


    // POST: Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.SignInAsync(model.Email, model.Password, model.RememberMe);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Errors?.FirstOrDefault() ?? "Email o contraseña inválidos.");
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            ModelState.AddModelError("", "Error inesperado: usuario no encontrado después del login.");
            return View(model);
        }

        var roles = await _userManager.GetRolesAsync(user);

        // Redirección por roles (prioridad sobre ReturnUrl)
        if (roles.Contains("Administrador"))
            return RedirectToAction("Index", "Admin");

        if (roles.Contains("Cliente"))
            return RedirectToAction("Index", "Client", new { area = "Client" });

        // Si existe ReturnUrl y es segura → úsala (solo si no tiene roles específicos)
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl != "/")
            return LocalRedirect(model.ReturnUrl);

        // Usuario sin rol conocido → inicio
        return RedirectToAction("Index", "Home");
    }


    // POST: Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // Access Denied
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}

