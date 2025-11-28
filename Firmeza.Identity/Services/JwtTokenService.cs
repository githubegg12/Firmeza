using Firmeza.Identity.DTOs;
using Firmeza.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Firmeza.Identity.Services;

/// <summary>
/// Service responsible for generating and managing JWT (JSON Web Token) authentication tokens.
/// Handles token creation with user claims and roles for API authentication.
/// </summary>
public class JwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the JwtTokenService class.
    /// </summary>
    /// <param name="jwtSettings">The JWT configuration settings from appsettings.json.</param>
    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Generates a JWT token for the specified user with their assigned roles.
    /// The token includes user ID, username, email, and role claims.
    /// </summary>
    /// <param name="user">The application user for whom to generate the token.</param>
    /// <param name="roles">The list of roles assigned to the user.</param>
    /// <returns>A signed JWT token string.</returns>
    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Calculates and returns the expiration time for a newly generated token.
    /// </summary>
    /// <returns>The DateTime when a new token would expire.</returns>
    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
    }
}
