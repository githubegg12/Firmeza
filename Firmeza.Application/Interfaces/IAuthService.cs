using System.Threading.Tasks;
using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registra un usuario y lo asigna a un rol.
    /// </summary>
    Task<AuthResult> RegisterAsync(string username, string email, string password, string role);

    /// <summary>
    /// Realiza el inicio de sesión con usuario y contraseña.
    /// </summary>
    Task<AuthResult> SignInAsync(string username, string password, bool rememberMe);

    /// <summary>
    /// Cierra la sesión del usuario actual.
    /// </summary>
    Task SignOutAsync();
}
