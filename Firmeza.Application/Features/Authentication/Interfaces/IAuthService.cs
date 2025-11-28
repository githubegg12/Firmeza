
using Firmeza.Application.DTOs;
using Firmeza.Application.DTOs.Client;

namespace Firmeza.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registra un usuario y lo asigna a un rol.
    /// </summary>
    Task<AuthResult> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Realiza el inicio de sesión con email y contraseña.
    /// </summary>
    Task<AuthResult> SignInAsync(string email, string password, bool rememberMe);

    /// <summary>
    /// Cierra la sesión del usuario actual.
    /// </summary>
    Task SignOutAsync();
}
