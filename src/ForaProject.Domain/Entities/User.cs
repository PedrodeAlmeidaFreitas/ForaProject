using ForaProject.Domain.Common;
using ForaProject.Domain.Exceptions;

namespace ForaProject.Domain.Entities;

/// <summary>
/// Represents a user in the system with authentication capabilities.
/// </summary>
public class User : Entity
{
    private readonly List<string> _roles = new();

    /// <summary>
    /// Gets the username (unique identifier for login).
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the email address (unique).
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the hashed password (BCrypt hash).
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the roles assigned to this user.
    /// </summary>
    public IReadOnlyCollection<string> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Gets a value indicating whether the user account is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the date when the user last logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    // Private constructor for EF Core
    private User() : base() { }

    /// <summary>
    /// Creates a new user with the specified credentials.
    /// </summary>
    /// <param name="username">The username (must be unique).</param>
    /// <param name="email">The email address (must be unique and valid).</param>
    /// <param name="passwordHash">The BCrypt hashed password.</param>
    /// <param name="roles">The initial roles to assign (optional).</param>
    /// <returns>A new User instance.</returns>
    /// <exception cref="InvalidUserDataException">Thrown when validation fails.</exception>
    public static User Create(string username, string email, string passwordHash, IEnumerable<string>? roles = null)
    {
        // Trim inputs before validation
        username = username?.Trim() ?? string.Empty;
        email = email?.Trim() ?? string.Empty;
        
        ValidateUsername(username);
        ValidateEmail(email);
        ValidatePasswordHash(passwordHash);

        var user = new User
        {
            Username = username,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            IsActive = true
        };

        if (roles != null)
        {
            foreach (var role in roles)
            {
                user.AddRole(role);
            }
        }

        return user;
    }

    /// <summary>
    /// Updates the user's email address.
    /// </summary>
    /// <param name="email">The new email address.</param>
    /// <exception cref="InvalidUserDataException">Thrown when email is invalid.</exception>
    public void UpdateEmail(string email)
    {
        ValidateEmail(email);
        Email = email.Trim().ToLowerInvariant();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the user's password hash.
    /// </summary>
    /// <param name="passwordHash">The new BCrypt hashed password.</param>
    /// <exception cref="InvalidUserDataException">Thrown when password hash is invalid.</exception>
    public void UpdatePassword(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);
        PasswordHash = passwordHash;
        MarkAsUpdated();
    }

    /// <summary>
    /// Adds a role to the user.
    /// </summary>
    /// <param name="role">The role to add (Admin, Manager, ReadOnly).</param>
    /// <exception cref="InvalidUserDataException">Thrown when role is invalid or already exists.</exception>
    public void AddRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidUserDataException("Role cannot be empty.");

        var normalizedRole = role.Trim();
        if (!IsValidRole(normalizedRole))
            throw new InvalidUserDataException($"Invalid role: {role}. Allowed roles: Admin, Manager, ReadOnly.");

        if (_roles.Contains(normalizedRole))
            throw new InvalidUserDataException($"User already has role: {role}.");

        _roles.Add(normalizedRole);
        MarkAsUpdated();
    }

    /// <summary>
    /// Removes a role from the user.
    /// </summary>
    /// <param name="role">The role to remove.</param>
    /// <exception cref="InvalidUserDataException">Thrown when role doesn't exist.</exception>
    public void RemoveRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidUserDataException("Role cannot be empty.");

        var normalizedRole = role.Trim();
        if (!_roles.Contains(normalizedRole))
            throw new InvalidUserDataException($"User does not have role: {role}.");

        _roles.Remove(normalizedRole);
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if the user has a specific role.
    /// </summary>
    /// <param name="role">The role to check.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    public bool HasRole(string role)
    {
        return _roles.Contains(role?.Trim() ?? string.Empty);
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Records a successful login.
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidUserDataException("Username cannot be empty.");

        if (username.Length < 3)
            throw new InvalidUserDataException("Username must be at least 3 characters long.");

        if (username.Length > 50)
            throw new InvalidUserDataException("Username cannot exceed 50 characters.");

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
            throw new InvalidUserDataException("Username can only contain letters, numbers, underscores, and hyphens.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidUserDataException("Email cannot be empty.");

        if (email.Length > 255)
            throw new InvalidUserDataException("Email cannot exceed 255 characters.");

        // Simple email validation
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailRegex))
            throw new InvalidUserDataException("Email format is invalid.");
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new InvalidUserDataException("Password hash cannot be empty.");

        // BCrypt hashes are 60 characters long and start with "$2" or "$2a" or "$2b" or "$2y"
        if (passwordHash.Length != 60 || !passwordHash.StartsWith("$2"))
            throw new InvalidUserDataException("Invalid password hash format.");
    }

    private static bool IsValidRole(string role)
    {
        var validRoles = new[] { "Admin", "Manager", "ReadOnly" };
        return validRoles.Contains(role);
    }
}
