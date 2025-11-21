namespace Firmeza.Identity.DTOs;

/// <summary>
/// DTO for authentication response containing JWT token
/// </summary>
public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime? Expiration { get; set; }
}
