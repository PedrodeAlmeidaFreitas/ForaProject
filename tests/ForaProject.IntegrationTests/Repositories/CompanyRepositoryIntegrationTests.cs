using FluentAssertions;
using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Entities;
using ForaProject.Domain.ValueObjects;
using ForaProject.Infrastructure.Data.Contexts;
using ForaProject.Infrastructure.Data.Repositories;
using ForaProject.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace ForaProject.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for CompanyRepository to increase infrastructure coverage.
/// </summary>
public class CompanyRepositoryIntegrationTests : IntegrationTestBase
{
    public CompanyRepositoryIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetAllWithIncomeRecordsAsync_ShouldReturnCompaniesWithIncome()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var company = Company.Create(CentralIndexKey.Create(1111), "Test Company");
        company.AddIncomeRecord(IncomeRecord.Create(company.Id, 2022, 100000m, "10-K", null, DateTime.UtcNow, "ACC"));
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllWithIncomeRecordsAsync();

        // Assert
        var companies = result.ToList();
        companies.Should().HaveCount(1);
        companies[0].IncomeRecords.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByCikWithIncomeRecordsAsync_WithExistingCik_ShouldReturnCompanyWithIncome()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var cik = CentralIndexKey.Create(2222);
        var company = Company.Create(cik, "CIK Test Company");
        company.AddIncomeRecord(IncomeRecord.Create(company.Id, 2022, 50000m, "10-K", null, DateTime.UtcNow, "ACC"));
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByCikWithIncomeRecordsAsync(cik);

        // Assert
        result.Should().NotBeNull();
        result!.EntityName.Should().Be("CIK Test Company");
        result.IncomeRecords.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByCikWithIncomeRecordsAsync_WithNonExistentCik_ShouldReturnNull()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var cik = CentralIndexKey.Create(9999999);

        // Act
        var result = await repository.GetByCikWithIncomeRecordsAsync(cik);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingCik_ShouldReturnTrue()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var cik = CentralIndexKey.Create(3333);
        var company = Company.Create(cik, "Exists Test");
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(cik);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentCik_ShouldReturnFalse()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var cik = CentralIndexKey.Create(9999999);

        // Act
        var result = await repository.ExistsAsync(cik);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetEligibleForFundingAsync_ShouldOnlyReturnCompaniesWithPositiveFundableAmounts()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        // Company with fundable amount
        var company1 = CreateTestCompany(4001, "Eligible Company",
            (2022, 120000m),
            (2021, 110000m),
            (2020, 100000m),
            (2019, 95000m),
            (2018, 90000m));

        // Company without fundable amount
        var company2 = Company.Create(CentralIndexKey.Create(4002), "Not Eligible");
        company2.AddIncomeRecord(IncomeRecord.Create(company2.Id, 2022, 10000m, "10-K", null, DateTime.UtcNow, "ACC"));

        await context.Companies.AddRangeAsync(company1, company2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetEligibleForFundingAsync();

        // Assert
        var eligible = result.ToList();
        eligible.Should().HaveCount(1);
        eligible[0].EntityName.Should().Be("Eligible Company");
        eligible[0].StandardFundableAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdWithIncomeRecordsAsync_ShouldReturnCompanyWithIncome()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var company = CreateTestCompany(5001, "ID Test Company",
            (2022, 75000m),
            (2021, 70000m));
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdWithIncomeRecordsAsync(company.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        result.IncomeRecords.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ShouldReturnSameCompany()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var company = Company.Create(CentralIndexKey.Create(6001), "Add Test");

        // Act
        await repository.AddAsync(company);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(company.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        result.EntityName.Should().Be("Add Test");
    }

    [Fact]
    public async Task Update_ShouldModifyCompany()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var company = Company.Create(CentralIndexKey.Create(7001), "Original Name");
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        // Modify the company (though EntityName is read-only in this case, 
        // we're testing the Update method itself)
        var retrieved = await repository.GetByIdAsync(company.Id);
        retrieved.Should().NotBeNull();

        // Act
        repository.Update(retrieved!);
        await context.SaveChangesAsync();

        // Assert
        var updated = await repository.GetByIdAsync(company.Id);
        updated.Should().NotBeNull();
        updated!.Id.Should().Be(company.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCompany()
    {
        // Arrange
        await ClearDatabaseAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = new CompanyRepository(context);

        var company = Company.Create(CentralIndexKey.Create(8001), "To Delete");
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(company.Id);
        await context.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(company.Id);
        result.Should().BeNull();
    }
}
