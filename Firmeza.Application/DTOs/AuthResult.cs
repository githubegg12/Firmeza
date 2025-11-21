namespace Firmeza.Application.DTOs;

/// <summary>
/// Resultado de operaciones de autenticación (login, registro)
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
