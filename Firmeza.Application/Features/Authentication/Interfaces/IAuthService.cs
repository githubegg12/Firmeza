
using Firmeza.Application.DTOs;
using Firmeza.Application.DTOs.Client;

namespace Firmeza.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user and assigns them to a role.
    /// </summary>
    Task<AuthResult> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Performs user login with email and password.
    /// </summary>
    Task<AuthResult> SignInAsync(string email, string password, bool rememberMe);

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    Task SignOutAsync();
}
