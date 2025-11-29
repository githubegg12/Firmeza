namespace Firmeza.Identity.DTOs;

/// <summary>
/// DTO for authentication response containing JWT token
/// </summary>
public class AuthResponse
{
    /// <summary>Indicates if authentication was successful</summary>
    public bool Success { get; set; }
    
    /// <summary>Message describing the result</summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>JWT token if successful</summary>
    public string? Token { get; set; }
    
    /// <summary>User ID</summary>
    public string? UserId { get; set; }
    
    /// <summary>User email</summary>
    public string? Email { get; set; }
    
    /// <summary>User full name</summary>
    public string? UserName { get; set; }
    
    /// <summary>List of roles assigned to the user</summary>
    public List<string> Roles { get; set; } = new();
    
    /// <summary>Token expiration date</summary>
    public DateTime? Expiration { get; set; }
}
