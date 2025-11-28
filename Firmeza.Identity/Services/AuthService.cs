using System.Security.Claims;
using Firmeza.Application.DTOs;
using Firmeza.Application.DTOs.Client;
using Firmeza.Domain.Entities;
using Firmeza.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Identity.Services;

/// <summary>
/// Service for handling user authentication and registration.
/// Implements the IAuthService interface and manages user accounts using ASP.NET Core Identity.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Initializes a new instance of the AuthService class.
    /// </summary>
    /// <param name="userManager">The UserManager for managing user accounts.</param>
    /// <param name="signInManager">The SignInManager for handling user sign-in operations.</param>
    /// <param name="roleManager">The RoleManager for managing user roles.</param>
    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var result = new AuthResult();

        // Use email as username
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
        {
            result.Success = false;
            result.Errors = new[] { "El email ya está en uso" };
            return result;
        }

        // Check for unique DocumentId
        var existingDocument = _userManager.Users.FirstOrDefault(u => u.DocumentId == request.DocumentId);
        if (existingDocument != null)
        {
            result.Success = false;
            result.Errors = new[] { "El documento de identidad ya está registrado" };
            return result;
        }

        var user = new ApplicationUser
        {
            UserName = request.Email, // Use email as username
            Email = request.Email,
            FullName = $"{request.FirstName} {request.LastName}", // Combine first and last name
            DocumentId = request.DocumentId,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            result.Success = false;
            result.Errors = createResult.Errors.Select(e => e.Description);
            return result;
        }

        // Assign role - all new registrations are automatically "Cliente"
        const string role = "Cliente";
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }
        await _userManager.AddToRoleAsync(user, role);

        result.Success = true;
        return result;
    }

    public async Task<AuthResult> SignInAsync(string email, string password, bool rememberMe)
    {
        var result = new AuthResult();

        // Find user by email instead of username
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            result.Success = false;
            result.Errors = new[] { "Email o contraseña inválidos" };
            return result;
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        if (!signInResult.Succeeded)
        {
            result.Success = false;
            result.Errors = new[] { "Email o contraseña inválidos" };
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
