using FluentAssertions;
using ForaProject.Domain.Entities;

namespace ForaProject.Domain.Tests.Entities;

/// <summary>
/// Unit tests for IncomeRecord entity.
/// </summary>
public class IncomeRecordTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnIncomeRecord()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var year = 2022;
        var amount = 1_000_000m;
        var form = "10-K";
        string? frame = null;
        var filedDate = new DateTime(2023, 2, 15);
        var accessionNumber = "0001234567-22-000001";

        // Act
        var record = IncomeRecord.Create(companyId, year, amount, form, frame, filedDate, accessionNumber);

        // Assert
        record.Should().NotBeNull();
        record.CompanyId.Should().Be(companyId);
        record.Year.Should().Be(year);
        record.Amount.Should().Be(amount);
        record.Form.Should().Be(form);
        record.Frame.Should().BeNull();
        record.FiledDate.Should().Be(filedDate);
        record.AccessionNumber.Should().Be(accessionNumber);
    }

    [Fact]
    public void Create_WithFrame_ShouldStoreFrame()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var frame = "CY2022";

        // Act
        var record = CreateTestRecord(companyId, frame: frame);

        // Assert
        record.Frame.Should().Be(frame);
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2050)]
    public void Create_WithInvalidYear_ShouldThrowArgumentException(int invalidYear)
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        Action act = () => IncomeRecord.Create(
            companyId,
            invalidYear,
            1_000_000m,
            "10-K",
            null,
            DateTime.UtcNow,
            "0001234567-22-000001");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*Invalid year: {invalidYear}*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidForm_ShouldThrowArgumentException(string? invalidForm)
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        Action act = () => IncomeRecord.Create(
            companyId,
            2022,
            1_000_000m,
            invalidForm!,
            null,
            DateTime.UtcNow,
            "0001234567-22-000001");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidAccessionNumber_ShouldThrowArgumentException(string? invalidAccessionNumber)
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        Action act = () => IncomeRecord.Create(
            companyId,
            2022,
            1_000_000m,
            "10-K",
            null,
            DateTime.UtcNow,
            invalidAccessionNumber!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(1_000_000)]
    [InlineData(0.01)]
    [InlineData(100)]
    public void IsPositiveIncome_WithPositiveAmount_ShouldReturnTrue(decimal positiveAmount)
    {
        // Arrange
        var record = CreateTestRecord(Guid.NewGuid(), amount: positiveAmount);

        // Act
        var result = record.IsPositiveIncome();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1_000_000)]
    [InlineData(-0.01)]
    public void IsPositiveIncome_WithNonPositiveAmount_ShouldReturnFalse(decimal nonPositiveAmount)
    {
        // Arrange
        var record = CreateTestRecord(Guid.NewGuid(), amount: nonPositiveAmount);

        // Act
        var result = record.IsPositiveIncome();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UpdateAmount_WithNewValue_ShouldUpdateAmount()
    {
        // Arrange
        var record = CreateTestRecord(Guid.NewGuid(), amount: 1_000_000m);
        var newAmount = 2_000_000m;

        // Act
        record.UpdateAmount(newAmount);

        // Assert
        record.Amount.Should().Be(newAmount);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldAllowNegativeValues()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var negativeAmount = -5_000_000m; // Losses are valid

        // Act
        var record = CreateTestRecord(companyId, amount: negativeAmount);

        // Assert
        record.Amount.Should().Be(negativeAmount);
        record.IsPositiveIncome().Should().BeFalse();
    }

    [Theory]
    [InlineData("10-K")]
    [InlineData("10-Q")]
    [InlineData("8-K")]
    public void Create_WithDifferentFormTypes_ShouldAcceptAllForms(string formType)
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        var record = CreateTestRecord(companyId, form: formType);

        // Assert
        record.Form.Should().Be(formType);
    }

    #region Helper Methods

    private static IncomeRecord CreateTestRecord(
        Guid companyId,
        int year = 2022,
        decimal amount = 1_000_000m,
        string form = "10-K",
        string? frame = null)
    {
        return IncomeRecord.Create(
            companyId: companyId,
            year: year,
            amount: amount,
            form: form,
            frame: frame,
            filedDate: new DateTime(year, 12, 31),
            accessionNumber: $"0001234567-{year}-000001");
    }

    #endregion
}
