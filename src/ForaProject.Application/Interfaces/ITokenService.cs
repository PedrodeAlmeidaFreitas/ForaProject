using ForaProject.Application.DTOs;

namespace ForaProject.Application.Interfaces;

/// <summary>
/// Service interface for JWT token generation and validation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user information.</param>
    /// <returns>The generated JWT token.</returns>
    string GenerateToken(UserDto user);

    /// <summary>
    /// Gets the token expiration time in seconds.
    /// </summary>
    /// <returns>The expiration time in seconds.</returns>
    int GetTokenExpirationSeconds();
}
