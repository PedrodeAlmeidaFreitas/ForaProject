using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Entities;
using ForaProject.Domain.ValueObjects;
using ForaProject.Infrastructure.Data.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace ForaProject.IntegrationTests.Fixtures;

/// <summary>
/// Base class for integration tests with database seeding capabilities.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Seeds the database with test data.
    /// </summary>
    protected async Task SeedDatabaseAsync(Action<ApplicationDbContext> seedAction)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clear existing data
        db.Companies.RemoveRange(db.Companies);
        await db.SaveChangesAsync();

        // Seed new data
        seedAction(db);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a sample company for testing.
    /// </summary>
    protected static Company CreateTestCompany(int cik, string entityName, params (int year, decimal amount)[] incomeData)
    {
        var company = Company.Create(
            CentralIndexKey.Create(cik),
            entityName);

        foreach (var (year, amount) in incomeData)
        {
            company.AddIncomeRecord(IncomeRecord.Create(
                company.Id, // Use the company's ID
                year,
                amount,
                "10-K",
                null,
                DateTime.UtcNow,
                $"ACC-{year}"));
        }

        // Calculate fundable amounts if there's income data
        if (incomeData.Length > 0)
        {
            company.CalculateFundableAmounts();
        }

        return company;
    }

    /// <summary>
    /// Clears the database.
    /// </summary>
    protected async Task ClearDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Remove all income records first (due to foreign key)
        db.IncomeRecords.RemoveRange(db.IncomeRecords);
        // Then remove companies
        db.Companies.RemoveRange(db.Companies);

        await db.SaveChangesAsync();

        // Clear change tracker to avoid tracking issues
        db.ChangeTracker.Clear();
    }
}
