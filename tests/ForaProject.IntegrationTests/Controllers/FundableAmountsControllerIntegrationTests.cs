using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ForaProject.Application.DTOs;
using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Entities;
using ForaProject.Domain.ValueObjects;
using ForaProject.IntegrationTests.Fixtures;

namespace ForaProject.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for FundableAmountsController endpoints.
/// </summary>
public class FundableAmountsControllerIntegrationTests : IntegrationTestBase
{
    public FundableAmountsControllerIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetFundableCompanies_WithNoCompanies_ShouldReturnEmptyList()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync("/api/v1/fundableamounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<FundableAmountDto>>();
        companies.Should().NotBeNull();
        companies.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFundableCompanies_WithCompaniesHavingIncomeData_ShouldReturnCalculatedAmounts()
    {
        // Arrange
        await SeedDatabaseAsync(db =>
        {
            // Company with standard fundable amount calculation (has all required years 2018-2022)
            db.Companies.Add(CreateTestCompany(1001, "Standard Company",
                (2022, 120000m),
                (2021, 110000m),
                (2020, 100000m),
                (2019, 95000m),
                (2018, 90000m)));

            // Company with growing revenue
            db.Companies.Add(CreateTestCompany(1002, "Growing Company",
                (2022, 200000m),
                (2021, 150000m),
                (2020, 120000m),
                (2019, 100000m),
                (2018, 80000m)));

            // Company with insufficient data (only 3 years, missing 2018-2019)
            db.Companies.Add(CreateTestCompany(1003, "New Company",
                (2022, 60000m),
                (2021, 55000m),
                (2020, 50000m)));
        });

        // Act
        var response = await Client.GetAsync("/api/v1/fundableamounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<FundableAmountDto>>();
        companies.Should().NotBeNull();
        companies.Should().HaveCount(2); // Only companies with complete data (2018-2022)

        var standardCompany = companies!.FirstOrDefault(c => c.Name == "Standard Company");
        standardCompany.Should().NotBeNull();
        standardCompany!.StandardFundableAmount.Should().BeGreaterThan(0);

        var growingCompany = companies.FirstOrDefault(c => c.Name == "Growing Company");
        growingCompany.Should().NotBeNull();
        growingCompany!.StandardFundableAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetFundableCompanies_WithStartsWithFilter_ShouldReturnOnlyMatchingCompanies()
    {
        // Arrange
        await SeedDatabaseAsync(db =>
        {
            // Company starting with H (complete data 2018-2022)
            db.Companies.Add(CreateTestCompany(2001, "High Income Company",
                (2022, 600000m),
                (2021, 550000m),
                (2020, 500000m),
                (2019, 480000m),
                (2018, 460000m)));

            // Company starting with L (complete data 2018-2022)
            db.Companies.Add(CreateTestCompany(2002, "Low Income Company",
                (2022, 12000m),
                (2021, 11000m),
                (2020, 10000m),
                (2019, 9500m),
                (2018, 9000m)));
        });

        // Act
        var response = await Client.GetAsync("/api/v1/fundableamounts?startsWith=H");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<FundableAmountDto>>();
        companies.Should().NotBeNull();
        companies.Should().HaveCount(1);
        companies![0].Name.Should().Be("High Income Company");
    }

    [Fact]
    public async Task GetFundableCompanies_WithStartsWithLetterEndpoint_ShouldReturnMatchingCompanies()
    {
        // Arrange
        await SeedDatabaseAsync(db =>
        {
            db.Companies.Add(CreateTestCompany(3001, "Alpha Company",
                (2022, 120000m),
                (2021, 110000m),
                (2020, 100000m),
                (2019, 95000m),
                (2018, 90000m)));

            db.Companies.Add(CreateTestCompany(3002, "Beta Company",
                (2022, 60000m),
                (2021, 55000m),
                (2020, 50000m),
                (2019, 48000m),
                (2018, 45000m)));
        });

        // Act
        var response = await Client.GetAsync("/api/v1/fundableamounts/letter/A");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<FundableAmountDto>>();
        companies.Should().NotBeNull();
        companies.Should().HaveCount(1);
        companies![0].Name.Should().Be("Alpha Company");
    }

    [Fact]
    public async Task GetFundableCompanies_WithInvalidStartsWith_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act - Using a number instead of a letter
        var response = await Client.GetAsync("/api/v1/fundableamounts?startsWith=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CalculateFundableAmountByCik_WithExistingCompany_ShouldReturnCalculatedAmount()
    {
        // Arrange
        const int testCik = 4001;
        await SeedDatabaseAsync(db =>
        {
            // Don't call CalculateFundableAmounts - the endpoint will do it
            var company = Company.Create(
                CentralIndexKey.Create(testCik),
                "Test Company");

            company.AddIncomeRecord(IncomeRecord.Create(company.Id, 2022, 120000m, "10-K", null, DateTime.UtcNow, "ACC-2022"));
            company.AddIncomeRecord(IncomeRecord.Create(company.Id, 2021, 110000m, "10-K", null, DateTime.UtcNow, "ACC-2021"));
            company.AddIncomeRecord(IncomeRecord.Create(company.Id, 2020, 100000m, "10-K", null, DateTime.UtcNow, "ACC-2020"));
            company.AddIncomeRecord(IncomeRecord.Create(company.Id, 2019, 95000m, "10-K", null, DateTime.UtcNow, "ACC-2019"));
            company.AddIncomeRecord(IncomeRecord.Create(company.Id, 2018, 90000m, "10-K", null, DateTime.UtcNow, "ACC-2018"));

            db.Companies.Add(company);
        });

        // Act - Use POST calculate endpoint
        var response = await Client.PostAsync($"/api/v1/fundableamounts/calculate/{testCik}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
        company.Should().NotBeNull();
        company!.Cik.Should().Be(testCik);
        company.EntityName.Should().Be("Test Company");
        company.StandardFundableAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CalculateFundableAmountByCik_WithNonExistentCik_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        const int nonExistentCik = 9999999;

        // Act - Use POST calculate endpoint
        var response = await Client.PostAsync($"/api/v1/fundableamounts/calculate/{nonExistentCik}", null);

        // Assert - InvalidCompanyDataException maps to BadRequest
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CalculateAllFundableAmounts_WithNoCompanies_ShouldReturnZeroCount()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.PostAsync("/api/v1/fundableamounts/calculate/all", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CalculateAllResult>();
        result.Should().NotBeNull();
        result!.ProcessedCount.Should().Be(0);
    }

    [Fact]
    public async Task CalculateAllFundableAmounts_WithMultipleCompanies_ShouldCalculateAll()
    {
        // Arrange
        await SeedDatabaseAsync(db =>
        {
            // Company 1 with complete data
            db.Companies.Add(CreateTestCompany(5001, "Company One",
                (2022, 150000m),
                (2021, 140000m),
                (2020, 130000m),
                (2019, 120000m),
                (2018, 110000m)));

            // Company 2 with complete data
            db.Companies.Add(CreateTestCompany(5002, "Company Two",
                (2022, 200000m),
                (2021, 190000m),
                (2020, 180000m),
                (2019, 170000m),
                (2018, 160000m)));

            // Company 3 with insufficient data (should get 0 amounts)
            var company3 = Company.Create(CentralIndexKey.Create(5003), "Company Three");
            company3.AddIncomeRecord(IncomeRecord.Create(company3.Id, 2022, 50000m, "10-K", null, DateTime.UtcNow, "ACC-2022"));
            db.Companies.Add(company3);
        });

        // Act
        var response = await Client.PostAsync("/api/v1/fundableamounts/calculate/all", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CalculateAllResult>();
        result.Should().NotBeNull();
        result!.ProcessedCount.Should().Be(3); // All 3 companies processed

        // Verify that companies now have calculated amounts
        var companiesResponse = await Client.GetAsync("/api/v1/fundableamounts");
        var companies = await companiesResponse.Content.ReadFromJsonAsync<List<FundableAmountDto>>();
        companies.Should().NotBeNull();
        companies!.Should().HaveCount(2); // Only 2 companies are eligible (have positive fundable amounts)
    }

    [Fact]
    public async Task GetFundableCompaniesByLetter_WithInvalidLetter_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act - Using a number instead of a letter
        var response = await Client.GetAsync("/api/v1/fundableamounts/letter/5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetFundableCompaniesByLetter_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        await SeedDatabaseAsync(db =>
        {
            db.Companies.Add(CreateTestCompany(6001, "Apple Inc",
                (2022, 100000m),
                (2021, 95000m),
                (2020, 90000m),
                (2019, 85000m),
                (2018, 80000m)));
        });

        // Act - Search for companies starting with Z (none exist)
        var response = await Client.GetAsync("/api/v1/fundableamounts/letter/Z");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<FundableAmountDto>>();
        companies.Should().NotBeNull();
        companies.Should().BeEmpty();
    }

    // Helper class for deserializing calculate/all response
    private class CalculateAllResult
    {
        public string Message { get; set; } = string.Empty;
        public int ProcessedCount { get; set; }
    }
}
