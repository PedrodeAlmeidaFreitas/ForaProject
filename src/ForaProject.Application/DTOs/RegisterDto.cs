namespace ForaProject.Application.DTOs;

/// <summary>
/// DTO for user registration requests.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the roles to assign to the user (optional, Admin only).
    /// </summary>
    public List<string>? Roles { get; set; }
}
