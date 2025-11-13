using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ForaProject.Application.DTOs;
using ForaProject.IntegrationTests.Fixtures;

namespace ForaProject.IntegrationTests.Filters;

/// <summary>
/// Integration tests for ValidationFilter.
/// Tests automatic model validation and error responses.
/// </summary>
public class ValidationFilterIntegrationTests : IntegrationTestBase
{
    public ValidationFilterIntegrationTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task ValidationFilter_WithInvalidCik_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidDto = new CreateCompanyDto
        {
            Cik = 0, // Invalid CIK (must be > 0)
            EntityName = "Test Company"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("CIK");
    }

    [Fact]
    public async Task ValidationFilter_WithEmptyEntityName_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidDto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = "" // Empty name
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("EntityName");
    }

    [Fact]
    public async Task ValidationFilter_WithNullEntityName_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidDto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = null! // Null name
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ValidationFilter_WithValidData_ShouldPass()
    {
        // Arrange
        await ClearDatabaseAsync();
        var validDto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = "Valid Company"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", validDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task ValidationFilter_WithLongEntityName_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidDto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = new string('A', 501) // Name longer than max length (500)
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("EntityName");
    }

    [Fact]
    public async Task ValidationFilter_ImportCompanyDto_WithInvalidCik_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidDto = new ImportCompanyDto
        {
            Cik = -1 // Negative CIK
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies/import", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ValidationFilter_BatchImportDto_WithEmptyList_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidDto = new BatchImportDto
        {
            Ciks = new List<int>() // Empty list
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies/import/batch", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ValidationFilter_BatchImportDto_WithInvalidCiks_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidDto = new BatchImportDto
        {
            Ciks = new List<int> { 1234567, 0, -1 } // Contains invalid CIKs
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies/import/batch", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ValidationFilter_FundableAmounts_WithInvalidStartsWith_ShouldReturnBadRequest()
    {
        // Arrange & Act - Using a special character instead of a letter
        var response = await Client.GetAsync("/api/v1/fundableamounts?startsWith=@");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
