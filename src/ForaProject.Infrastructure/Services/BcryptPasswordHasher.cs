using ForaProject.Application.Interfaces;

namespace ForaProject.Infrastructure.Services;

/// <summary>
/// BCrypt password hashing service implementation.
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
