using FluentAssertions;
using FluentValidation.TestHelper;
using ForaProject.Application.DTOs;
using ForaProject.Application.Validators;
using Xunit;

namespace ForaProject.Application.Tests.Validators;

public class LoginDtoValidatorTests
{
    private readonly LoginDtoValidator _validator;

    public LoginDtoValidatorTests()
    {
        _validator = new LoginDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyEmail_ShouldHaveError(string emptyEmail)
    {
        // Arrange
        var dto = new LoginDto { Email = emptyEmail, Password = "Password123!" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@nodomain.com")]
    [InlineData("user@")]
    public void Validate_WithInvalidEmailFormat_ShouldHaveError(string invalidEmail)
    {
        // Arrange
        var dto = new LoginDto { Email = invalidEmail, Password = "Password123!" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithLongEmail_ShouldHaveError()
    {
        // Arrange - create email that's too long but still valid format
        var longEmail = "user" + new string('a', 240) + "@example.com"; // 260 characters
        var dto = new LoginDto { Email = longEmail, Password = "Password123!" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyPassword_ShouldHaveError(string emptyPassword)
    {
        // Arrange
        var dto = new LoginDto { Email = "test@example.com", Password = emptyPassword };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldHaveError()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@example.com", Password = "short" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }
}
