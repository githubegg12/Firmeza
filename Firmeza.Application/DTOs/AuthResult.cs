namespace Firmeza.Application.DTOs;

/// <summary>
/// Result of authentication operations (login, registration)
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// List of error messages if the operation failed
    /// </summary>
    public IEnumerable<string>? Errors { get; set; }
}
