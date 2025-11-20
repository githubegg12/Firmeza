using System.Security.Claims;
using Firmeza.Application.DTOs;
using Firmeza.Identity.Entities;
using Firmeza.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<AuthResult> RegisterAsync(string username, string email, string password, string role)
    {
        var result = new AuthResult();

        var existing = await _userManager.FindByNameAsync(username);
        if (existing != null)
        {
            result.Success = false;
            result.Errors = new[] { "El nombre de usuario ya está en uso" };
            return result;
        }

        var user = new ApplicationUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            result.Success = false;
            result.Errors = createResult.Errors.Select(e => e.Description);
            return result;
        }

        // Ensure role exists
        if (!string.IsNullOrWhiteSpace(role))
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
            await _userManager.AddToRoleAsync(user, role);
        }

        result.Success = true;
        return result;
    }

    public async Task<AuthResult> SignInAsync(string username, string password, bool rememberMe)
    {
        var result = new AuthResult();

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            result.Success = false;
            result.Errors = new[] { "Usuario o contraseña inválidos" };
            return result;
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        if (!signInResult.Succeeded)
        {
            result.Success = false;
            result.Errors = new[] { "Usuario o contraseña inválidos" };
            return result;
        }

        result.Success = true;
        return result;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
    {
        return _signInManager.Context.AuthenticateAsync(scheme);
    }

    public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        return _signInManager.Context.ChallengeAsync(scheme, properties);
    }

    public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        return _signInManager.Context.ForbidAsync(scheme, properties);
    }

    public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        return _signInManager.Context.SignInAsync(scheme, principal, properties);
    }

    public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        return _signInManager.Context.SignOutAsync(scheme, properties);
    }
}
