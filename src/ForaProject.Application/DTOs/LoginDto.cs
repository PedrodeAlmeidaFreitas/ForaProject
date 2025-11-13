namespace ForaProject.Application.DTOs;

/// <summary>
/// DTO for user login requests.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
