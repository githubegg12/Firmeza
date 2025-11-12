using Firmeza.Application.Interfaces;
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

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl ?? "/" });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Attempt to sign in using the authentication service
        var result = await _authService.SignInAsync(model.Username, model.Password, model.RememberMe);
        if (!result.Success)
        {
            // If login fails, show the first error
            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "Error al iniciar sesión");
            return View(model);
        }

        // Find the user to check roles
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // Redirect based on role
            if (roles.Contains("Administrador"))
                return RedirectToAction("Index", "Admin"); // Admin dashboard
            if (roles.Contains("Cliente"))
                return RedirectToAction("Index", "Client"); // Client dashboard
        }

        // If user has no recognized role, redirect to ReturnUrl if present
        if (!string.IsNullOrEmpty(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);

        // Fallback to Home if no ReturnUrl or role matched
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Por defecto registramos como Cliente
        var role = string.IsNullOrEmpty(model.Role) ? "Cliente" : model.Role;
        var result = await _authService.RegisterAsync(model.Username, model.Email, model.Password, role);
        if (!result.Success)
        {
            foreach (var err in result.Errors ?? Enumerable.Empty<string>())
                ModelState.AddModelError(string.Empty, err);
            return View(model);
        }

        // Login automático tras registro
        await _authService.SignInAsync(model.Username, model.Password, false);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
