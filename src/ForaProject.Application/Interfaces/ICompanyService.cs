using ForaProject.Application.DTOs;

namespace ForaProject.Application.Interfaces;

/// <summary>
/// Service interface for company-related operations.
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Gets all companies.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of company DTOs.</returns>
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a company by its CIK.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The company DTO if found; otherwise, null.</returns>
    Task<CompanyDto?> GetCompanyByCikAsync(int cik, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a company by its ID.
    /// </summary>
    /// <param name="id">The company ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The company DTO if found; otherwise, null.</returns>
    Task<CompanyDto?> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new company.
    /// </summary>
    /// <param name="createCompanyDto">The company data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created company DTO.</returns>
    Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports company data from SEC EDGAR API.
    /// </summary>
    /// <param name="importDto">The import data containing CIK.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The imported company DTO.</returns>
    Task<CompanyDto> ImportCompanyAsync(ImportCompanyDto importDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports multiple companies from SEC EDGAR API.
    /// </summary>
    /// <param name="batchImportDto">The batch import data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of imported company DTOs.</returns>
    Task<IEnumerable<CompanyDto>> BatchImportCompaniesAsync(BatchImportDto batchImportDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a company by its ID.
    /// </summary>
    /// <param name="id">The company ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteCompanyAsync(Guid id, CancellationToken cancellationToken = default);
}
