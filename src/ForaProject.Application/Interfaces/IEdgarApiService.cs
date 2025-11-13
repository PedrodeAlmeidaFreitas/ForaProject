namespace ForaProject.Application.Interfaces;

/// <summary>
/// Service interface for interacting with SEC EDGAR API.
/// </summary>
public interface IEdgarApiService
{
    /// <summary>
    /// Fetches company data from SEC EDGAR API.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The EDGAR API response data.</returns>
    Task<EdgarCompanyData?> GetCompanyDataAsync(int cik, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents company data from SEC EDGAR API.
/// </summary>
public class EdgarCompanyData
{
    public int Cik { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public List<EdgarIncomeData> IncomeRecords { get; set; } = new();
}

/// <summary>
/// Represents income data from SEC EDGAR API.
/// </summary>
public class EdgarIncomeData
{
    public int Year { get; set; }
    public decimal Amount { get; set; }
    public string Form { get; set; } = string.Empty;
    public string? Frame { get; set; }
    public DateTime FiledDate { get; set; }
    public string AccessionNumber { get; set; } = string.Empty;
}
