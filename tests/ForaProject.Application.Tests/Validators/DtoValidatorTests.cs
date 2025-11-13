using FluentAssertions;
using FluentValidation.TestHelper;
using ForaProject.Application.DTOs;
using ForaProject.Application.Validators;

namespace ForaProject.Application.Tests.Validators;

/// <summary>
/// Unit tests for ImportCompanyDtoValidator.
/// </summary>
public class ImportCompanyDtoValidatorTests
{
    private readonly ImportCompanyDtoValidator _validator;

    public ImportCompanyDtoValidatorTests()
    {
        _validator = new ImportCompanyDtoValidator();
    }

    [Fact]
    public void Validate_WithValidCik_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new ImportCompanyDto { Cik = 1234567 };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cik);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-12345)]
    public void Validate_WithInvalidCik_ShouldHaveValidationError(int invalidCik)
    {
        // Arrange
        var dto = new ImportCompanyDto { Cik = invalidCik };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cik);
    }
}

/// <summary>
/// Unit tests for CreateCompanyDtoValidator.
/// </summary>
public class CreateCompanyDtoValidatorTests
{
    private readonly CreateCompanyDtoValidator _validator;

    public CreateCompanyDtoValidatorTests()
    {
        _validator = new CreateCompanyDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = "Test Company Inc."
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidCik_ShouldHaveValidationError(int invalidCik)
    {
        // Arrange
        var dto = new CreateCompanyDto
        {
            Cik = invalidCik,
            EntityName = "Test Company"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cik);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidEntityName_ShouldHaveValidationError(string? invalidName)
    {
        // Arrange
        var dto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = invalidName!
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EntityName);
    }
}

/// <summary>
/// Unit tests for BatchImportDtoValidator.
/// </summary>
public class BatchImportDtoValidatorTests
{
    private readonly BatchImportDtoValidator _validator;

    public BatchImportDtoValidatorTests()
    {
        _validator = new BatchImportDtoValidator();
    }

    [Fact]
    public void Validate_WithValidCiks_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new BatchImportDto
        {
            Ciks = new List<int> { 1234567, 7654321, 1111111 }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyList_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new BatchImportDto { Ciks = new List<int>() };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ciks);
    }

    [Fact]
    public void Validate_WithNullList_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new BatchImportDto { Ciks = null! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ciks);
    }

    [Fact]
    public void Validate_WithInvalidCikInList_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new BatchImportDto
        {
            Ciks = new List<int> { 1234567, -1, 7654321 }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ciks);
    }
}
