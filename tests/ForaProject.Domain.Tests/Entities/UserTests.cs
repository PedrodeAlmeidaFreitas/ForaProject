using FluentAssertions;
using ForaProject.Domain.Entities;
using ForaProject.Domain.Exceptions;
using Xunit;

namespace ForaProject.Domain.Tests.Entities;

public class UserTests
{
    // Valid BCrypt hash (exactly 60 characters) - $2a$ + cost(2) + $ + salt(22) + hash(31) = 60 chars
    private const string ValidPasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy";

    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var passwordHash = ValidPasswordHash;

        // Act
        var user = User.Create(username, email, passwordHash);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Username.Should().Be(username);
        user.Email.Should().Be(email.ToLowerInvariant());
        user.PasswordHash.Should().Be(passwordHash);
        user.IsActive.Should().BeTrue();
        user.Roles.Should().BeEmpty();
        user.LastLoginAt.Should().BeNull();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithRoles_ShouldAssignRoles()
    {
        // Arrange
        var roles = new[] { "Admin", "Manager" };

        // Act
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash, roles);

        // Assert
        user.Roles.Should().HaveCount(2);
        user.Roles.Should().Contain("Admin");
        user.Roles.Should().Contain("Manager");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyUsername_ShouldThrowException(string? invalidUsername)
    {
        // Act
        var act = () => User.Create(invalidUsername!, "test@example.com", ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Username cannot be empty.");
    }

    [Fact]
    public void Create_WithNullUsername_ShouldThrowException()
    {
        // Act
        var act = () => User.Create(null!, "test@example.com", ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Username cannot be empty.");
    }

    [Theory]
    [InlineData("ab")] // Too short
    public void Create_WithShortUsername_ShouldThrowException(string shortUsername)
    {
        // Act
        var act = () => User.Create(shortUsername, "test@example.com", ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Username must be at least 3 characters long.");
    }

    [Fact]
    public void Create_WithLongUsername_ShouldThrowException()
    {
        // Arrange
        var longUsername = new string('a', 51); // 51 characters

        // Act
        var act = () => User.Create(longUsername, "test@example.com", ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Username cannot exceed 50 characters.");
    }

    [Theory]
    [InlineData("user name")] // Contains space
    [InlineData("user@name")] // Contains @
    [InlineData("user.name")] // Contains dot
    public void Create_WithInvalidUsernameCharacters_ShouldThrowException(string invalidUsername)
    {
        // Act
        var act = () => User.Create(invalidUsername, "test@example.com", ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Username can only contain letters, numbers, underscores, and hyphens.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyEmail_ShouldThrowException(string? invalidEmail)
    {
        // Act
        var act = () => User.Create("testuser", invalidEmail!, ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Email cannot be empty.");
    }

    [Fact]
    public void Create_WithNullEmail_ShouldThrowException()
    {
        // Act
        var act = () => User.Create("testuser", null!, ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Email cannot be empty.");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@domain")]
    [InlineData("@nodomain.com")]
    [InlineData("user @domain.com")]
    public void Create_WithInvalidEmailFormat_ShouldThrowException(string invalidEmail)
    {
        // Act
        var act = () => User.Create("testuser", invalidEmail, ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Email format is invalid.");
    }

    [Fact]
    public void Create_WithLongEmail_ShouldThrowException()
    {
        // Arrange
        var longEmail = new string('a', 247) + "@test.com"; // 256 characters (247 + 9)

        // Act
        var act = () => User.Create("testuser", longEmail, ValidPasswordHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Email cannot exceed 255 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyPasswordHash_ShouldThrowException(string? invalidHash)
    {
        // Act
        var act = () => User.Create("testuser", "test@example.com", invalidHash!);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Password hash cannot be empty.");
    }

    [Fact]
    public void Create_WithNullPasswordHash_ShouldThrowException()
    {
        // Act
        var act = () => User.Create("testuser", "test@example.com", null!);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Password hash cannot be empty.");
    }

    [Theory]
    [InlineData("tooshort")] // Not 60 characters
    [InlineData("$1$tooshortforvalidbcrypthash123456789012345678901234567890")] // Wrong prefix
    public void Create_WithInvalidPasswordHash_ShouldThrowException(string invalidHash)
    {
        // Act
        var act = () => User.Create("testuser", "test@example.com", invalidHash);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Invalid password hash format.");
    }

    [Fact]
    public void AddRole_WithValidRole_ShouldAddRole()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        user.AddRole("Admin");

        // Assert
        user.Roles.Should().Contain("Admin");
        user.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Manager")]
    [InlineData("ReadOnly")]
    public void AddRole_WithAllValidRoles_ShouldSucceed(string role)
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        user.AddRole(role);

        // Assert
        user.Roles.Should().Contain(role);
    }

    [Theory]
    [InlineData("InvalidRole")]
    [InlineData("SuperAdmin")]
    [InlineData("Guest")]
    public void AddRole_WithInvalidRole_ShouldThrowException(string invalidRole)
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        var act = () => user.AddRole(invalidRole);

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage($"Invalid role: {invalidRole}. Allowed roles: Admin, Manager, ReadOnly.");
    }

    [Fact]
    public void AddRole_WithDuplicateRole_ShouldThrowException()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash, new[] { "Admin" });

        // Act
        var act = () => user.AddRole("Admin");

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("User already has role: Admin.");
    }

    [Fact]
    public void RemoveRole_WithExistingRole_ShouldRemoveRole()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash, new[] { "Admin", "Manager" });

        // Act
        user.RemoveRole("Admin");

        // Assert
        user.Roles.Should().NotContain("Admin");
        user.Roles.Should().Contain("Manager");
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void RemoveRole_WithNonExistingRole_ShouldThrowException()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        var act = () => user.RemoveRole("Admin");

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("User does not have role: Admin.");
    }

    [Fact]
    public void HasRole_WithExistingRole_ShouldReturnTrue()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash, new[] { "Admin" });

        // Act
        var result = user.HasRole("Admin");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasRole_WithNonExistingRole_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        var result = user.HasRole("Admin");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);
        user.Deactivate();

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void RecordLogin_ShouldSetLastLoginAt()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        user.RecordLogin();

        // Assert
        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);
        var newEmail = "newemail@example.com";

        // Act
        user.UpdateEmail(newEmail);

        // Assert
        user.Email.Should().Be(newEmail.ToLowerInvariant());
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateEmail_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        var act = () => user.UpdateEmail("invalidemail");

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Email format is invalid.");
    }

    [Fact]
    public void UpdatePassword_WithValidPasswordHash_ShouldUpdatePassword()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);
        var newPasswordHash = "$2a$11$X9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy";

        // Act
        user.UpdatePassword(newPasswordHash);

        // Assert
        user.PasswordHash.Should().Be(newPasswordHash);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdatePassword_WithInvalidPasswordHash_ShouldThrowException()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", ValidPasswordHash);

        // Act
        var act = () => user.UpdatePassword("invalidhash");

        // Assert
        act.Should().Throw<InvalidUserDataException>()
            .WithMessage("Invalid password hash format.");
    }

    [Fact]
    public void Create_EmailShouldBeLowercase()
    {
        // Arrange
        var mixedCaseEmail = "Test@Example.COM";

        // Act
        var user = User.Create("testuser", mixedCaseEmail, ValidPasswordHash);

        // Assert
        user.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_UsernameShouldBeTrimmed()
    {
        // Arrange
        var usernameWithSpaces = "  testuser  ";

        // Act
        var user = User.Create(usernameWithSpaces, "test@example.com", ValidPasswordHash);

        // Assert
        user.Username.Should().Be("testuser");
    }
}
