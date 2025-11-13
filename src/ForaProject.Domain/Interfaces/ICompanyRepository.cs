using ForaProject.Domain.Aggregates;
using ForaProject.Domain.ValueObjects;

namespace ForaProject.Domain.Interfaces;

/// <summary>
/// Repository interface for Company aggregate operations.
/// </summary>
public interface ICompanyRepository : IRepository<Company>
{
    /// <summary>
    /// Gets a company by its CIK.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The company if found; otherwise, null.</returns>
    Task<Company?> GetByCikAsync(CentralIndexKey cik, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a company by its CIK including income records.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The company with income records if found; otherwise, null.</returns>
    Task<Company?> GetByCikWithIncomeRecordsAsync(CentralIndexKey cik, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all companies including income records.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All companies with income records.</returns>
    Task<IEnumerable<Company>> GetAllWithIncomeRecordsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets companies eligible for funding.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of eligible companies.</returns>
    Task<IEnumerable<Company>> GetEligibleForFundingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets companies whose names start with the specified letter.
    /// </summary>
    /// <param name="letter">The starting letter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of matching companies.</returns>
    Task<IEnumerable<Company>> GetByNameStartsWithAsync(char letter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a company with the specified CIK exists.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(CentralIndexKey cik, CancellationToken cancellationToken = default);
}
