using ForaProject.Domain.Common;
using ForaProject.Domain.Entities;
using ForaProject.Domain.Exceptions;
using ForaProject.Domain.ValueObjects;

namespace ForaProject.Domain.Aggregates;

/// <summary>
/// Represents a company in the system.
/// Aggregate root for company-related operations and fundable amount calculations.
/// </summary>
public sealed class Company : Entity
{
    private readonly List<IncomeRecord> _incomeRecords = new();

    /// <summary>
    /// Gets the Central Index Key (CIK) for this company.
    /// </summary>
    public CentralIndexKey Cik { get; private set; }

    /// <summary>
    /// Gets the company's legal entity name.
    /// </summary>
    public string EntityName { get; private set; }

    /// <summary>
    /// Gets the standard fundable amount.
    /// </summary>
    public decimal? StandardFundableAmount { get; private set; }

    /// <summary>
    /// Gets the special fundable amount (with adjustments).
    /// </summary>
    public decimal? SpecialFundableAmount { get; private set; }

    /// <summary>
    /// Gets the collection of income records for this company.
    /// </summary>
    public IReadOnlyCollection<IncomeRecord> IncomeRecords => _incomeRecords.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Company"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private Company() : base()
    {
        Cik = null!;
        EntityName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Company"/> class.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="entityName">The company name.</param>
    private Company(CentralIndexKey cik, string entityName) : base()
    {
        Cik = cik ?? throw new ArgumentNullException(nameof(cik));
        EntityName = entityName ?? throw new ArgumentNullException(nameof(entityName));

        ValidateInvariants();
    }

    /// <summary>
    /// Creates a new company.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="entityName">The company name.</param>
    /// <returns>A new Company instance.</returns>
    public static Company Create(CentralIndexKey cik, string entityName)
    {
        return new Company(cik, entityName);
    }

    /// <summary>
    /// Adds an income record to the company.
    /// </summary>
    /// <param name="incomeRecord">The income record to add.</param>
    public void AddIncomeRecord(IncomeRecord incomeRecord)
    {
        if (incomeRecord == null)
            throw new ArgumentNullException(nameof(incomeRecord));

        if (incomeRecord.CompanyId != Id)
            throw new InvalidCompanyDataException("Income record belongs to a different company.");

        // Avoid duplicates based on year and form
        var existing = _incomeRecords.FirstOrDefault(ir =>
            ir.Year == incomeRecord.Year && ir.Form == incomeRecord.Form);

        if (existing != null)
        {
            _incomeRecords.Remove(existing);
        }

        _incomeRecords.Add(incomeRecord);
        MarkAsUpdated();
    }

    /// <summary>
    /// Calculates and sets the fundable amounts for this company.
    /// </summary>
    /// <exception cref="InsufficientIncomeDataException">Thrown when data requirements are not met.</exception>
    public void CalculateFundableAmounts()
    {
        // Requirement 1: Must have income data for all years 2018-2022
        var requiredYears = new[] { 2018, 2019, 2020, 2021, 2022 };
        var availableYears = _incomeRecords.Select(ir => ir.Year).Distinct().ToArray();

        if (!requiredYears.All(year => availableYears.Contains(year)))
        {
            StandardFundableAmount = 0;
            SpecialFundableAmount = 0;
            return;
        }

        // Requirement 2: Must have positive income in both 2021 and 2022
        var income2021 = _incomeRecords.FirstOrDefault(ir => ir.Year == 2021)?.Amount ?? 0;
        var income2022 = _incomeRecords.FirstOrDefault(ir => ir.Year == 2022)?.Amount ?? 0;

        if (income2021 <= 0 || income2022 <= 0)
        {
            StandardFundableAmount = 0;
            SpecialFundableAmount = 0;
            return;
        }

        // Calculate Standard Fundable Amount
        var highestIncome = _incomeRecords
            .Where(ir => requiredYears.Contains(ir.Year))
            .Max(ir => ir.Amount);

        const decimal tenBillion = 10_000_000_000m;
        var standardPercentage = highestIncome >= tenBillion ? 0.1233m : 0.2151m;
        StandardFundableAmount = Math.Round(highestIncome * standardPercentage, 2);

        // Calculate Special Fundable Amount
        var specialAmount = StandardFundableAmount.Value;

        // Add 15% if company name starts with vowel
        if (StartsWithVowel(EntityName))
        {
            specialAmount *= 1.15m;
        }

        // Subtract 25% if 2022 income < 2021 income
        if (income2022 < income2021)
        {
            specialAmount *= 0.75m;
        }

        SpecialFundableAmount = Math.Round(specialAmount, 2);
        MarkAsUpdated();
    }

    /// <summary>
    /// Determines whether the company name starts with a vowel.
    /// </summary>
    /// <param name="name">The company name.</param>
    /// <returns>True if starts with vowel; otherwise, false.</returns>
    private static bool StartsWithVowel(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var firstChar = char.ToUpperInvariant(name.TrimStart()[0]);
        return firstChar is 'A' or 'E' or 'I' or 'O' or 'U';
    }

    /// <summary>
    /// Gets whether the company is eligible for funding.
    /// </summary>
    /// <returns>True if eligible; otherwise, false.</returns>
    public bool IsEligibleForFunding()
    {
        return StandardFundableAmount.HasValue &&
               StandardFundableAmount.Value > 0;
    }

    /// <summary>
    /// Validates domain invariants.
    /// </summary>
    private void ValidateInvariants()
    {
        if (string.IsNullOrWhiteSpace(EntityName))
            throw new InvalidCompanyDataException("Entity name cannot be empty.");
    }

    /// <summary>
    /// Updates the company name.
    /// </summary>
    /// <param name="entityName">The new entity name.</param>
    public void UpdateEntityName(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            throw new InvalidCompanyDataException("Entity name cannot be empty.");

        EntityName = entityName;
        MarkAsUpdated();

        // Recalculate special amount if standard amount exists (name affects calculation)
        if (StandardFundableAmount.HasValue)
        {
            CalculateFundableAmounts();
        }
    }
}
