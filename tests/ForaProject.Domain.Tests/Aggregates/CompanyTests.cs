using FluentAssertions;
using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Entities;
using ForaProject.Domain.Exceptions;
using ForaProject.Domain.ValueObjects;

namespace ForaProject.Domain.Tests.Aggregates;

/// <summary>
/// Unit tests for Company aggregate.
/// </summary>
public class CompanyTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnCompany()
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);
        var entityName = "Test Company Inc.";

        // Act
        var company = Company.Create(cik, entityName);

        // Assert
        company.Should().NotBeNull();
        company.Cik.Should().Be(cik);
        company.EntityName.Should().Be(entityName);
        company.IncomeRecords.Should().BeEmpty();
        company.StandardFundableAmount.Should().BeNull();
        company.SpecialFundableAmount.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullCik_ShouldThrowArgumentNullException()
    {
        // Arrange
        CentralIndexKey? cik = null;
        var entityName = "Test Company Inc.";

        // Act
        Action act = () => Company.Create(cik!, entityName);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("cik");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidEntityName_ShouldThrowException(string? invalidName)
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);

        // Act
        Action act = () => Company.Create(cik, invalidName!);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithNullEntityName_ShouldThrowException()
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);

        // Act
        Action act = () => Company.Create(cik, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("entityName");
    }

    [Fact]
    public void AddIncomeRecord_WithValidRecord_ShouldAddToCollection()
    {
        // Arrange
        var company = CreateTestCompany();
        var incomeRecord = CreateIncomeRecord(company.Id, 2022, 1_000_000m);

        // Act
        company.AddIncomeRecord(incomeRecord);

        // Assert
        company.IncomeRecords.Should().HaveCount(1);
        company.IncomeRecords.Should().Contain(incomeRecord);
    }

    [Fact]
    public void AddIncomeRecord_WithDuplicateYearAndForm_ShouldReplacePrevious()
    {
        // Arrange
        var company = CreateTestCompany();
        var record1 = CreateIncomeRecord(company.Id, 2022, 1_000_000m);
        var record2 = CreateIncomeRecord(company.Id, 2022, 2_000_000m);

        // Act
        company.AddIncomeRecord(record1);
        company.AddIncomeRecord(record2);

        // Assert
        company.IncomeRecords.Should().HaveCount(1);
        company.IncomeRecords.First().Amount.Should().Be(2_000_000m);
    }

    [Fact]
    public void CalculateFundableAmounts_WithoutAllRequiredYears_ShouldSetAmountsToZero()
    {
        // Arrange
        var company = CreateTestCompany();
        company.AddIncomeRecord(CreateIncomeRecord(company.Id, 2021, 1_000_000m));
        company.AddIncomeRecord(CreateIncomeRecord(company.Id, 2022, 1_100_000m));

        // Act
        company.CalculateFundableAmounts();

        // Assert
        company.StandardFundableAmount.Should().Be(0);
        company.SpecialFundableAmount.Should().Be(0);
    }

    [Fact]
    public void CalculateFundableAmounts_WithNegativeIncomeIn2021_ShouldSetAmountsToZero()
    {
        // Arrange
        var company = CreateTestCompany();
        AddAllYearsIncomeRecords(company,
            year2018: 1_000_000m,
            year2019: 1_100_000m,
            year2020: 1_200_000m,
            year2021: -500_000m,  // Negative!
            year2022: 1_300_000m);

        // Act
        company.CalculateFundableAmounts();

        // Assert
        company.StandardFundableAmount.Should().Be(0);
        company.SpecialFundableAmount.Should().Be(0);
    }

    [Fact]
    public void CalculateFundableAmounts_WithNegativeIncomeIn2022_ShouldSetAmountsToZero()
    {
        // Arrange
        var company = CreateTestCompany();
        AddAllYearsIncomeRecords(company,
            year2018: 1_000_000m,
            year2019: 1_100_000m,
            year2020: 1_200_000m,
            year2021: 1_300_000m,
            year2022: -500_000m);  // Negative!

        // Act
        company.CalculateFundableAmounts();

        // Assert
        company.StandardFundableAmount.Should().Be(0);
        company.SpecialFundableAmount.Should().Be(0);
    }

    [Fact]
    public void CalculateFundableAmounts_WithIncomeLessThan10B_ShouldUse21Point51Percent()
    {
        // Arrange
        var company = CreateTestCompany();
        var highestIncome = 5_000_000_000m; // $5B - less than $10B
        AddAllYearsIncomeRecords(company,
            year2018: 3_000_000_000m,
            year2019: 3_500_000_000m,
            year2020: 4_000_000_000m,
            year2021: 4_500_000_000m,
            year2022: highestIncome);

        // Act
        company.CalculateFundableAmounts();

        // Assert
        var expectedStandard = Math.Round(highestIncome * 0.2151m, 2);
        company.StandardFundableAmount.Should().Be(expectedStandard);
        company.SpecialFundableAmount.Should().Be(expectedStandard); // No special adjustments
    }

    [Fact]
    public void CalculateFundableAmounts_WithIncomeGreaterThan10B_ShouldUse12Point33Percent()
    {
        // Arrange
        var company = CreateTestCompany();
        var highestIncome = 15_000_000_000m; // $15B - greater than $10B
        AddAllYearsIncomeRecords(company,
            year2018: 10_000_000_000m,
            year2019: 11_000_000_000m,
            year2020: 12_000_000_000m,
            year2021: 13_000_000_000m,
            year2022: highestIncome);

        // Act
        company.CalculateFundableAmounts();

        // Assert
        var expectedStandard = Math.Round(highestIncome * 0.1233m, 2);
        company.StandardFundableAmount.Should().Be(expectedStandard);
        company.SpecialFundableAmount.Should().Be(expectedStandard); // No special adjustments
    }

    [Theory]
    [InlineData("AMETEK INC/")]      // Starts with A
    [InlineData("EDISON INTERNATIONAL")] // Starts with E
    [InlineData("Intel Corporation")]    // Starts with I
    [InlineData("Oracle Corporation")]   // Starts with O
    [InlineData("Uber Technologies")]    // Starts with U
    public void CalculateFundableAmounts_WithVowelStart_ShouldAdd15Percent(string companyName)
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);
        var company = Company.Create(cik, companyName);
        var highestIncome = 1_000_000_000m;
        AddAllYearsIncomeRecords(company,
            year2018: 800_000_000m,
            year2019: 850_000_000m,
            year2020: 900_000_000m,
            year2021: 950_000_000m,
            year2022: highestIncome);

        // Act
        company.CalculateFundableAmounts();

        // Assert
        var expectedStandard = Math.Round(highestIncome * 0.2151m, 2);
        var expectedSpecial = Math.Round(expectedStandard * 1.15m, 2);
        company.StandardFundableAmount.Should().Be(expectedStandard);
        company.SpecialFundableAmount.Should().Be(expectedSpecial);
    }

    [Theory]
    [InlineData("Microsoft Corporation")]
    [InlineData("Tesla Inc")]
    [InlineData("Google LLC")]
    public void CalculateFundableAmounts_WithoutVowelStart_ShouldNotAdd15Percent(string companyName)
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);
        var company = Company.Create(cik, companyName);
        var highestIncome = 1_000_000_000m;
        AddAllYearsIncomeRecords(company,
            year2018: 800_000_000m,
            year2019: 850_000_000m,
            year2020: 900_000_000m,
            year2021: 950_000_000m,
            year2022: highestIncome);

        // Act
        company.CalculateFundableAmounts();

        // Assert
        var expectedStandard = Math.Round(highestIncome * 0.2151m, 2);
        company.StandardFundableAmount.Should().Be(expectedStandard);
        company.SpecialFundableAmount.Should().Be(expectedStandard); // Same as standard
    }

    [Fact]
    public void CalculateFundableAmounts_WithIncomeDecreaseIn2022_ShouldSubtract25Percent()
    {
        // Arrange
        var company = CreateTestCompany();
        AddAllYearsIncomeRecords(company,
            year2018: 800_000_000m,
            year2019: 850_000_000m,
            year2020: 900_000_000m,
            year2021: 1_000_000_000m,  // Higher
            year2022: 900_000_000m);   // Lower than 2021

        // Act
        company.CalculateFundableAmounts();

        // Assert
        var expectedStandard = Math.Round(1_000_000_000m * 0.2151m, 2);
        var expectedSpecial = Math.Round(expectedStandard * 0.75m, 2); // -25%
        company.StandardFundableAmount.Should().Be(expectedStandard);
        company.SpecialFundableAmount.Should().Be(expectedSpecial);
    }

    [Fact]
    public void CalculateFundableAmounts_WithVowelAndDecrease_ShouldApplyBothAdjustments()
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);
        var company = Company.Create(cik, "Apple Inc"); // Starts with vowel
        AddAllYearsIncomeRecords(company,
            year2018: 800_000_000m,
            year2019: 850_000_000m,
            year2020: 900_000_000m,
            year2021: 1_000_000_000m,  // Higher
            year2022: 900_000_000m);   // Lower

        // Act
        company.CalculateFundableAmounts();

        // Assert
        var expectedStandard = Math.Round(1_000_000_000m * 0.2151m, 2);
        var expectedSpecial = Math.Round(expectedStandard * 1.15m * 0.75m, 2); // +15% then -25%
        company.StandardFundableAmount.Should().Be(expectedStandard);
        company.SpecialFundableAmount.Should().Be(expectedSpecial);
    }

    [Fact]
    public void IsEligibleForFunding_WithPositiveFundableAmount_ShouldReturnTrue()
    {
        // Arrange
        var company = CreateTestCompany();
        AddAllYearsIncomeRecords(company,
            year2018: 800_000_000m,
            year2019: 850_000_000m,
            year2020: 900_000_000m,
            year2021: 950_000_000m,
            year2022: 1_000_000_000m);
        company.CalculateFundableAmounts();

        // Act
        var result = company.IsEligibleForFunding();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEligibleForFunding_WithoutCalculation_ShouldReturnFalse()
    {
        // Arrange
        var company = CreateTestCompany();

        // Act
        var result = company.IsEligibleForFunding();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UpdateEntityName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var company = CreateTestCompany();
        var newName = "Updated Company Name LLC";

        // Act
        company.UpdateEntityName(newName);

        // Assert
        company.EntityName.Should().Be(newName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateEntityName_WithInvalidName_ShouldThrowException(string? invalidName)
    {
        // Arrange
        var company = CreateTestCompany();

        // Act
        Action act = () => company.UpdateEntityName(invalidName!);

        // Assert
        act.Should().Throw<InvalidCompanyDataException>()
            .WithMessage("*Entity name cannot be empty*");
    }

    [Fact]
    public void UpdateEntityName_WithNull_ShouldThrowException()
    {
        // Arrange
        var company = CreateTestCompany();

        // Act
        Action act = () => company.UpdateEntityName(null!);

        // Assert
        act.Should().Throw<InvalidCompanyDataException>()
            .WithMessage("*Entity name cannot be empty*");
    }

    [Fact]
    public void UpdateEntityName_AfterCalculation_ShouldRecalculateFundableAmounts()
    {
        // Arrange
        var company = CreateTestCompany();
        AddAllYearsIncomeRecords(company,
            year2018: 800_000_000m,
            year2019: 850_000_000m,
            year2020: 900_000_000m,
            year2021: 950_000_000m,
            year2022: 1_000_000_000m);
        company.CalculateFundableAmounts();
        var originalSpecial = company.SpecialFundableAmount;

        // Act - Change to name starting with vowel
        company.UpdateEntityName("Apple Inc");

        // Assert
        company.SpecialFundableAmount.Should().NotBe(originalSpecial);
        company.SpecialFundableAmount.Should().BeGreaterThan(company.StandardFundableAmount!.Value);
    }

    #region Helper Methods

    private static Company CreateTestCompany()
    {
        var cik = CentralIndexKey.Create(1234567);
        return Company.Create(cik, "Test Company Inc.");
    }

    private static IncomeRecord CreateIncomeRecord(Guid companyId, int year, decimal amount)
    {
        return IncomeRecord.Create(
            companyId: companyId,
            year: year,
            amount: amount,
            form: "10-K",
            frame: null,
            filedDate: new DateTime(year, 12, 31),
            accessionNumber: $"0001234567-{year}-000001");
    }

    private static void AddAllYearsIncomeRecords(
        Company company,
        decimal year2018,
        decimal year2019,
        decimal year2020,
        decimal year2021,
        decimal year2022)
    {
        company.AddIncomeRecord(CreateIncomeRecord(company.Id, 2018, year2018));
        company.AddIncomeRecord(CreateIncomeRecord(company.Id, 2019, year2019));
        company.AddIncomeRecord(CreateIncomeRecord(company.Id, 2020, year2020));
        company.AddIncomeRecord(CreateIncomeRecord(company.Id, 2021, year2021));
        company.AddIncomeRecord(CreateIncomeRecord(company.Id, 2022, year2022));
    }

    #endregion
}
