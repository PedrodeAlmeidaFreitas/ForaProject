using ForaProject.Application.DTOs;

namespace ForaProject.Application.Interfaces;

/// <summary>
/// Service interface for authentication and user management operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A token response containing the JWT and user information.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid or user is inactive.</exception>
    Task<TokenResponseDto> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="registerDto">The registration information.</param>
    /// <returns>A token response containing the JWT and user information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when username or email already exists.</exception>
    Task<TokenResponseDto> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Gets the current user's information by ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The user information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user is not found.</exception>
    Task<UserDto> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="currentPassword">The current password.</param>
    /// <param name="newPassword">The new password.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when current password is incorrect.</exception>
    /// <exception cref="InvalidOperationException">Thrown when user is not found.</exception>
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}
