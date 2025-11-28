using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Email.Interfaces;
using Firmeza.Application.DTOs;
using Firmeza.Application.DTOs.Client;
using Firmeza.Identity.DTOs;
using Firmeza.Domain.Entities;
using Firmeza.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.API.Controllers;

/// <summary>
/// Controller for authentication operations (login, register)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwtTokenService,
        IEmailService emailService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// User login endpoint - returns JWT token on successful authentication
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            
            // Restrict "Administrador" from logging into the Client App (API)
            // Admins should only use the Admin Panel (Web App)
            if (roles.Contains("Administrador"))
            {
                _logger.LogWarning("Admin user {Email} attempted to login to Client App", request.Email);
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Acceso denegado. Los administradores deben usar el panel de administración."
                });
            }

            var token = _jwtTokenService.GenerateToken(user, roles);
            var expiration = _jwtTokenService.GetTokenExpiration();

            _logger.LogInformation("User {Email} logged in successfully", request.Email);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login exitoso",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList(),
                Expiration = expiration
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "Error interno del servidor"
            });
        }
    }

    /// <summary>
    /// User registration endpoint - creates new user with Cliente role by default
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "El email ya está registrado"
                });
            }


            // Check for unique DocumentId
            var existingDocument = _userManager.Users.FirstOrDefault(u => u.DocumentId == request.DocumentId);
            if (existingDocument != null)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "El documento de identidad ya está registrado"
                });
            }

            // Create new user entity
            var user = new ApplicationUser
            {
                UserName = request.Email, // Use email as username
                Email = request.Email,
                FullName = $"{request.FirstName} {request.LastName}", // Combine first and last name
                DocumentId = request.DocumentId,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                EmailConfirmed = true // Auto-confirm for simplicity
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = $"Error al crear usuario: {errors}"
                });
            }

            // Assign role - all new registrations are automatically "Cliente"
            const string role = "Cliente";
            await _userManager.AddToRoleAsync(user, role);

            // Send welcome email (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email!, user.UserName!);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
                }
            });

            // Generate token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenService.GenerateToken(user, roles);
            var expiration = _jwtTokenService.GetTokenExpiration();

            _logger.LogInformation("New user registered: {Email} with role {Role}", request.Email, role);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Registro exitoso",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList(),
                Expiration = expiration
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", request.Email);
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "Error interno del servidor"
            });
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> GetCurrentUser()
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Email = user.Email,
            UserName = user.UserName,
            Roles = roles.ToList()
        });
    }
}
