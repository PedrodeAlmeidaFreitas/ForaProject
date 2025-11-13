using ForaProject.Domain.Entities;

namespace ForaProject.Domain.Interfaces;

/// <summary>
/// Repository interface for User aggregate operations.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Checks if a user with the specified email already exists.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <returns>True if a user exists with the email; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Checks if a user with the specified username already exists.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if a user exists with the username; otherwise, false.</returns>
    Task<bool> ExistsByUsernameAsync(string username);

    /// <summary>
    /// Gets all active users.
    /// </summary>
    /// <returns>A collection of active users.</returns>
    Task<IEnumerable<User>> GetActiveUsersAsync();

    /// <summary>
    /// Gets all users with a specific role.
    /// </summary>
    /// <param name="role">The role to filter by.</param>
    /// <returns>A collection of users with the specified role.</returns>
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
}
