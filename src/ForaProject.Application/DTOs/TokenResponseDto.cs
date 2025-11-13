namespace ForaProject.Application.DTOs;

/// <summary>
/// DTO for authentication token responses.
/// </summary>
public class TokenResponseDto
{
    /// <summary>
    /// Gets or sets the JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token type (always "Bearer").
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Gets or sets the token expiration time in seconds.
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the user information.
    /// </summary>
    public UserDto User { get; set; } = null!;
}
