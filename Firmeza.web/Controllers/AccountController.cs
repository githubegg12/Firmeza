using Firmeza.Application.Interfaces;
using Firmeza.Identity.Entities;
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

        var result = await _authService.SignInAsync(model.Username, model.Password, false);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Errors?.FirstOrDefault() ?? "Invalid username or password.");
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null)
        {
            ModelState.AddModelError("", "Unexpected error: user not found after login.");
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

    // GET: Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    // POST: Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // SECURITY FIX: Always force "Cliente" role for new registrations.
        // Ignore model.Role to prevent privilege escalation.
        var role = "Cliente";

        var result = await _authService.RegisterAsync(model.Username, model.Email, model.Password, role);

        if (!result.Success)
        {
            foreach (var error in result.Errors ?? Enumerable.Empty<string>())
                ModelState.AddModelError("", error);

            return View(model);
        }

        // Auto login después del registro
        await _authService.SignInAsync(model.Username, model.Password, false);

        return RedirectToAction("Index", "Client");
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

