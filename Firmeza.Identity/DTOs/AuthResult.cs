namespace Firmeza.Identity.DTOs;

public class AuthResult
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}