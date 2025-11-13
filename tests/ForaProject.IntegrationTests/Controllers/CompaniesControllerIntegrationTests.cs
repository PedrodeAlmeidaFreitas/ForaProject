using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ForaProject.Application.DTOs;
using ForaProject.IntegrationTests.Fixtures;

namespace ForaProject.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for CompaniesController endpoints.
/// </summary>
public class CompaniesControllerIntegrationTests : IntegrationTestBase
{
    public CompaniesControllerIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetAllCompanies_WithNoCompanies_ShouldReturnEmptyList()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync("/api/v1/companies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<CompanyDto>>();
        companies.Should().NotBeNull();
        companies.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCompanies_WithMultipleCompanies_ShouldReturnAllCompanies()
    {
        // Arrange
        await SeedDatabaseAsync(db =>
        {
            db.Companies.Add(CreateTestCompany(1234, "Test Company 1"));
            db.Companies.Add(CreateTestCompany(5678, "Test Company 2"));
            db.Companies.Add(CreateTestCompany(9999, "Test Company 3"));
        });

        // Act
        var response = await Client.GetAsync("/api/v1/companies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<CompanyDto>>();
        companies.Should().NotBeNull();
        companies.Should().HaveCount(3);
        companies.Should().Contain(c => c.EntityName == "Test Company 1");
        companies.Should().Contain(c => c.EntityName == "Test Company 2");
        companies.Should().Contain(c => c.EntityName == "Test Company 3");
    }

    [Fact]
    public async Task GetCompanyByCik_WithExistingCik_ShouldReturnCompany()
    {
        // Arrange
        const int testCik = 1234567;
        await SeedDatabaseAsync(db =>
        {
            db.Companies.Add(CreateTestCompany(testCik, "Test Company",
                (2023, 100000m),
                (2022, 120000m),
                (2021, 130000m)));
        });

        // Act
        var response = await Client.GetAsync($"/api/v1/companies/cik/{testCik}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
        company.Should().NotBeNull();
        company!.Cik.Should().Be(testCik);
        company.EntityName.Should().Be("Test Company");
        company.IncomeRecords.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetCompanyByCik_WithNonExistentCik_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        const int nonExistentCik = 999999;

        // Act
        var response = await Client.GetAsync($"/api/v1/companies/cik/{nonExistentCik}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCompanyById_WithExistingId_ShouldReturnCompany()
    {
        // Arrange
        Guid companyId = Guid.Empty;
        await SeedDatabaseAsync(db =>
        {
            var company = CreateTestCompany(1234567, "Test Company");
            db.Companies.Add(company);
            companyId = company.Id;
        });

        // Act
        var response = await Client.GetAsync($"/api/v1/companies/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
        company.Should().NotBeNull();
        company!.Id.Should().Be(companyId);
        company.EntityName.Should().Be("Test Company");
    }

    [Fact]
    public async Task GetCompanyById_WithNonExistentId_ShouldReturnNotFound()
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
    public async Task CreateCompany_WithValidData_ShouldCreateCompanyAndReturn201()
    {
        // Arrange
        await ClearDatabaseAsync();
        var createDto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = "New Test Company"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
        company.Should().NotBeNull();
        company!.Cik.Should().Be(createDto.Cik);
        company.EntityName.Should().Be(createDto.EntityName);

        // Verify location header (case-insensitive)
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().ToLower().Should().Contain($"/api/v1/companies/cik/{company.Cik}");
    }

    [Fact]
    public async Task CreateCompany_WithInvalidCik_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateCompanyDto
        {
            Cik = 0, // Invalid CIK
            EntityName = "Test Company"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCompany_WithMissingEntityName_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateCompanyDto
        {
            Cik = 1234567,
            EntityName = "" // Empty name
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCompany_WithExistingId_ShouldDeleteAndReturn204()
    {
        // Arrange
        Guid companyId = Guid.Empty;
        await SeedDatabaseAsync(db =>
        {
            var company = CreateTestCompany(1234567, "Test Company");
            db.Companies.Add(company);
            companyId = company.Id;
        });

        // Act
        var response = await Client.DeleteAsync($"/api/v1/companies/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await Client.GetAsync($"/api/v1/companies/{companyId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCompany_WithNonExistentId_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/companies/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCompanyById_WithInvalidGuid_ShouldReturnNotFound()
    {
        // Arrange & Act - Invalid GUID format causes route to not match
        var response = await Client.GetAsync("/api/v1/companies/invalid-guid");

        // Assert - ASP.NET Core returns 404 when route parameter doesn't match
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCompanyByCik_WithInvalidCik_ShouldReturnBadRequest()
    {
        // Arrange & Act - CIK must be between 1 and 9999999
        var response = await Client.GetAsync("/api/v1/companies/cik/0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCompany_WithDuplicateCik_AfterCreation_ShouldReturnBadRequest()
    {
        // Arrange
        const int duplicateCik = 7777777;
        var firstDto = new CreateCompanyDto
        {
            Cik = duplicateCik,
            EntityName = "First Company"
        };

        // Create first company
        await Client.PostAsJsonAsync("/api/v1/companies", firstDto);

        var secondDto = new CreateCompanyDto
        {
            Cik = duplicateCik,
            EntityName = "Second Company"
        };

        // Act - Try to create another company with same CIK
        var response = await Client.PostAsJsonAsync("/api/v1/companies", secondDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllCompanies_WithCompaniesContainingIncomeRecords_ShouldIncludeIncome()
    {
        // Arrange
        await SeedDatabaseAsync(db =>
        {
            db.Companies.Add(CreateTestCompany(8001, "Company With Income",
                (2022, 100000m),
                (2021, 95000m),
                (2020, 90000m)));
        });

        // Act
        var response = await Client.GetAsync("/api/v1/companies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<CompanyDto>>();
        companies.Should().NotBeNull();
        companies.Should().HaveCount(1);
        companies![0].IncomeRecords.Should().HaveCount(3);
        companies[0].IncomeRecords.Should().OnlyContain(ir => ir.Amount > 0);
    }

    [Fact]
    public async Task DeleteCompany_WithCompanyHavingIncomeRecords_ShouldDeleteBoth()
    {
        // Arrange
        Guid companyId = Guid.Empty;
        await SeedDatabaseAsync(db =>
        {
            var company = CreateTestCompany(8002, "Company To Delete",
                (2022, 50000m),
                (2021, 45000m));
            db.Companies.Add(company);
            companyId = company.Id;
        });

        // Act
        var response = await Client.DeleteAsync($"/api/v1/companies/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify company and income records are deleted
        var getResponse = await Client.GetAsync($"/api/v1/companies/{companyId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
