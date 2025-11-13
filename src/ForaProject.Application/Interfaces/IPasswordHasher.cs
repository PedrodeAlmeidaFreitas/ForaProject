namespace ForaProject.Application.Interfaces;

/// <summary>
/// Service interface for password hashing operations.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password using BCrypt.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>The BCrypt hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies that a plain text password matches a hashed password.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="passwordHash">The BCrypt hashed password.</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    bool VerifyPassword(string password, string passwordHash);
}
