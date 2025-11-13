using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ForaProject.Application.DTOs;
using ForaProject.IntegrationTests.Fixtures;

namespace ForaProject.IntegrationTests.Middleware;

/// <summary>
/// Integration tests for ExceptionHandlingMiddleware.
/// Tests global exception handling and error responses.
/// </summary>
public class ExceptionHandlingMiddlewareIntegrationTests : IntegrationTestBase
{
    public ExceptionHandlingMiddlewareIntegrationTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Middleware_WhenDomainExceptionThrown_ShouldReturn400WithErrorMessage()
    {
        // Arrange
        await ClearDatabaseAsync();
        var invalidDto = new CreateCompanyDto
        {
            Cik = 0, // Invalid CIK will trigger validation
            EntityName = "Test Company"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Middleware_WhenResourceNotFound_ShouldReturn404()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/v1/companies/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Middleware_WhenValidationFails_ShouldReturn400()
    {
        // Arrange
        var invalidDto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = "" // Empty name should fail validation
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Middleware_WhenDuplicateCompanyCreated_ShouldReturn400()
    {
        // Arrange
        const int duplicateCik = 8001;
        await SeedDatabaseAsync(db =>
        {
            db.Companies.Add(CreateTestCompany(duplicateCik, "Existing Company"));
        });

        var duplicateDto = new CreateCompanyDto
        {
            Cik = duplicateCik,
            EntityName = "Duplicate Company"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", duplicateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("already exists");
    }
}
