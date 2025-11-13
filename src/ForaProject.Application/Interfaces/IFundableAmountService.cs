using ForaProject.Application.DTOs;

namespace ForaProject.Application.Interfaces;

/// <summary>
/// Service interface for fundable amount calculations and queries.
/// </summary>
public interface IFundableAmountService
{
    /// <summary>
    /// Gets all companies eligible for funding.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of fundable amount DTOs.</returns>
    Task<IEnumerable<FundableAmountDto>> GetFundableCompaniesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets fundable companies whose names start with the specified letter.
    /// </summary>
    /// <param name="startsWith">The starting letter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of fundable amount DTOs.</returns>
    Task<IEnumerable<FundableAmountDto>> GetFundableCompaniesByLetterAsync(char startsWith, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates fundable amounts for a specific company.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated company DTO with calculated amounts.</returns>
    Task<CompanyDto> CalculateFundableAmountAsync(int cik, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates fundable amounts for all companies.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of companies processed.</returns>
    Task<int> CalculateAllFundableAmountsAsync(CancellationToken cancellationToken = default);
}
