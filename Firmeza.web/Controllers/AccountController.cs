using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Email.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers;

/// <summary>
/// Handles user authentication for the admin panel
/// Manages login, logout, and access denied scenarios
/// </summary>
public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(IAuthService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays the login page
    /// </summary>
    /// <param name="returnUrl">URL to redirect to after successful login</param>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"),
        });
    }


    /// <summary>
    /// Processes login attempt and redirects based on user role
    /// Administrators go to Admin dashboard, Clients to Client area
    /// </summary>
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

        // Role-based redirection (takes priority over ReturnUrl)
        if (roles.Contains("Administrador"))
            return RedirectToAction("Index", "Admin");

        if (roles.Contains("Cliente"))
            return RedirectToAction("Index", "Client", new { area = "Client" });

        // If ReturnUrl exists and is safe, use it (only if no specific role)
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl != "/")
            return LocalRedirect(model.ReturnUrl);

        // User without known role → home page
        return RedirectToAction("Index", "Home");
    }


    /// <summary>
    /// Logs out the current user and redirects to home page
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Displays access denied page when user lacks required permissions
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}

