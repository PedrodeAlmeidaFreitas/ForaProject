using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Common;

namespace ForaProject.Domain.Entities;

/// <summary>
/// Represents a company's income record for a specific year.
/// Contains financial data extracted from SEC filings.
/// </summary>
public sealed class IncomeRecord : Entity
{
    /// <summary>
    /// Gets the ID of the company this record belongs to.
    /// </summary>
    public Guid CompanyId { get; private set; }

    /// <summary>
    /// Gets the year of this income record.
    /// </summary>
    public int Year { get; private set; }

    /// <summary>
    /// Gets the income/loss amount in USD.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Gets the SEC form type (e.g., 10-K, 10-Q, 8-K).
    /// </summary>
    public string Form { get; private set; }

    /// <summary>
    /// Gets the time frame identifier (e.g., CY2021). May be null for annual reports.
    /// </summary>
    public string? Frame { get; private set; }

    /// <summary>
    /// Gets the date when the filing was submitted.
    /// </summary>
    public DateTime FiledDate { get; private set; }

    /// <summary>
    /// Gets the SEC accession number.
    /// </summary>
    public string AccessionNumber { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncomeRecord"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private IncomeRecord() : base()
    {
        Form = string.Empty;
        Frame = null;
        AccessionNumber = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncomeRecord"/> class.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="year">The year.</param>
    /// <param name="amount">The income amount.</param>
    /// <param name="form">The SEC form type.</param>
    /// <param name="frame">The time frame (may be null for annual reports).</param>
    /// <param name="filedDate">The filing date.</param>
    /// <param name="accessionNumber">The accession number.</param>
    private IncomeRecord(
        Guid companyId,
        int year,
        decimal amount,
        string form,
        string? frame,
        DateTime filedDate,
        string accessionNumber) : base()
    {
        CompanyId = companyId;
        Year = year;
        Amount = amount;
        Form = form ?? throw new ArgumentNullException(nameof(form));
        Frame = frame; // Frame can be null for annual reports
        FiledDate = filedDate;
        AccessionNumber = accessionNumber ?? throw new ArgumentNullException(nameof(accessionNumber));

        ValidateInvariants();
    }

    /// <summary>
    /// Creates a new income record.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="year">The year.</param>
    /// <param name="amount">The income amount.</param>
    /// <param name="form">The SEC form type.</param>
    /// <param name="frame">The time frame.</param>
    /// <param name="filedDate">The filing date.</param>
    /// <param name="accessionNumber">The accession number.</param>
    /// <returns>A new IncomeRecord instance.</returns>
    public static IncomeRecord Create(
        Guid companyId,
        int year,
        decimal amount,
        string form,
        string? frame,
        DateTime filedDate,
        string accessionNumber)
    {
        return new IncomeRecord(companyId, year, amount, form, frame, filedDate, accessionNumber);
    }

    /// <summary>
    /// Updates the income amount.
    /// </summary>
    /// <param name="amount">The new amount.</param>
    public void UpdateAmount(decimal amount)
    {
        Amount = amount;
        MarkAsUpdated();
    }

    /// <summary>
    /// Validates domain invariants.
    /// </summary>
    private void ValidateInvariants()
    {
        if (Year < 1900 || Year > DateTime.UtcNow.Year)
            throw new ArgumentException($"Invalid year: {Year}", nameof(Year));

        if (string.IsNullOrWhiteSpace(Form))
            throw new ArgumentException("Form cannot be empty.", nameof(Form));

        // Frame is optional (can be null for annual 10-K filings)

        if (string.IsNullOrWhiteSpace(AccessionNumber))
            throw new ArgumentException("Accession number cannot be empty.", nameof(AccessionNumber));
    }

    /// <summary>
    /// Determines whether this is a positive income (not a loss).
    /// </summary>
    /// <returns>True if amount is positive; otherwise, false.</returns>
    public bool IsPositiveIncome() => Amount > 0;
}
