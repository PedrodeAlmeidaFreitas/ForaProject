using ForaProject.Application.DTOs;
using ForaProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ForaProject.API.Controllers;

/// <summary>
/// Controller for managing companies.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(
        ICompanyService companyService,
        ILogger<CompaniesController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all companies.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of companies.</returns>
    /// <response code="200">Returns the list of companies.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies");
        var companies = await _companyService.GetAllCompaniesAsync(cancellationToken);
        return Ok(companies);
    }

    /// <summary>
    /// Gets a company by CIK.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The company if found.</returns>
    /// <response code="200">Returns the company.</response>
    /// <response code="404">If the company is not found.</response>
    [HttpGet("cik/{cik:int}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> GetCompanyByCik(int cik, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company with CIK: {Cik}", cik);
        var company = await _companyService.GetCompanyByCikAsync(cik, cancellationToken);

        if (company == null)
        {
            return NotFound(new { message = $"Company with CIK {cik} not found." });
        }

        return Ok(company);
    }

    /// <summary>
    /// Gets a company by ID.
    /// </summary>
    /// <param name="id">The company ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The company if found.</returns>
    /// <response code="200">Returns the company.</response>
    /// <response code="404">If the company is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> GetCompanyById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company with ID: {Id}", id);
        var company = await _companyService.GetCompanyByIdAsync(id, cancellationToken);

        if (company == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        return Ok(company);
    }

    /// <summary>
    /// Creates a new company manually.
    /// </summary>
    /// <param name="createCompanyDto">The company data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created company.</returns>
    /// <response code="201">Returns the newly created company.</response>
    /// <response code="400">If the data is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompanyDto>> CreateCompany(
        [FromBody] CreateCompanyDto createCompanyDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating company with CIK: {Cik}", createCompanyDto.Cik);
        var company = await _companyService.CreateCompanyAsync(createCompanyDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetCompanyByCik),
            new { cik = company.Cik },
            company);
    }

    /// <summary>
    /// Imports a company from SEC EDGAR API.
    /// </summary>
    /// <param name="importDto">The import data containing CIK.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The imported company.</returns>
    /// <response code="201">Returns the imported company.</response>
    /// <response code="400">If the data is invalid or company not found in SEC.</response>
    [HttpPost("import")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompanyDto>> ImportCompany(
        [FromBody] ImportCompanyDto importDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Importing company with CIK: {Cik}", importDto.Cik);
        var company = await _companyService.ImportCompanyAsync(importDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetCompanyByCik),
            new { cik = company.Cik },
            company);
    }

    /// <summary>
    /// Imports multiple companies from SEC EDGAR API.
    /// </summary>
    /// <param name="batchImportDto">The batch import data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The imported companies.</returns>
    /// <response code="200">Returns the imported companies.</response>
    /// <response code="400">If the data is invalid.</response>
    [HttpPost("import/batch")]
    [ProducesResponseType(typeof(IEnumerable<CompanyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> BatchImportCompanies(
        [FromBody] BatchImportDto batchImportDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Batch importing {Count} companies", batchImportDto.Ciks.Count);
        var companies = await _companyService.BatchImportCompaniesAsync(batchImportDto, cancellationToken);

        return Ok(companies);
    }

    /// <summary>
    /// Deletes a company by ID.
    /// </summary>
    /// <param name="id">The company ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the company was deleted successfully.</response>
    /// <response code="404">If the company is not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCompany(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting company with ID: {Id}", id);
        await _companyService.DeleteCompanyAsync(id, cancellationToken);

        return NoContent();
    }
}
