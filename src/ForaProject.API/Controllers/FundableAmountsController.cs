using ForaProject.Application.DTOs;
using ForaProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ForaProject.API.Controllers;

/// <summary>
/// Controller for managing fundable amounts.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class FundableAmountsController : ControllerBase
{
    private readonly IFundableAmountService _fundableAmountService;
    private readonly ILogger<FundableAmountsController> _logger;

    public FundableAmountsController(
        IFundableAmountService fundableAmountService,
        ILogger<FundableAmountsController> logger)
    {
        _fundableAmountService = fundableAmountService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all companies eligible for funding.
    /// Optionally filter by company name starting letter.
    /// </summary>
    /// <param name="startsWith">Optional: Filter companies whose names start with this letter (A-Z).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of fundable companies.</returns>
    /// <response code="200">Returns the list of fundable companies.</response>
    /// <response code="400">If the startsWith parameter is invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FundableAmountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FundableAmountDto>>> GetFundableCompanies(
        [FromQuery] char? startsWith,
        CancellationToken cancellationToken)
    {
        // Validate optional filter parameter
        if (startsWith.HasValue && !char.IsLetter(startsWith.Value))
        {
            return BadRequest(new { message = "startsWith parameter must be a valid letter (A-Z)." });
        }

        if (startsWith.HasValue)
        {
            _logger.LogInformation("Getting fundable companies starting with letter: {Letter}", startsWith.Value);
            var filteredCompanies = await _fundableAmountService.GetFundableCompaniesByLetterAsync(startsWith.Value, cancellationToken);
            return Ok(filteredCompanies);
        }

        _logger.LogInformation("Getting all fundable companies");
        var companies = await _fundableAmountService.GetFundableCompaniesAsync(cancellationToken);
        return Ok(companies);
    }

    /// <summary>
    /// Gets fundable companies whose names start with the specified letter.
    /// </summary>
    /// <param name="letter">The starting letter (A-Z).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of filtered fundable companies.</returns>
    /// <response code="200">Returns the filtered list of fundable companies.</response>
    /// <response code="400">If the letter parameter is invalid.</response>
    [HttpGet("letter/{letter}")]
    [ProducesResponseType(typeof(IEnumerable<FundableAmountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FundableAmountDto>>> GetFundableCompaniesByLetter(
        char letter,
        CancellationToken cancellationToken)
    {
        if (!char.IsLetter(letter))
        {
            return BadRequest(new { message = "Letter parameter must be a valid letter (A-Z)." });
        }

        _logger.LogInformation("Getting fundable companies starting with letter: {Letter}", letter);
        var companies = await _fundableAmountService.GetFundableCompaniesByLetterAsync(letter, cancellationToken);
        return Ok(companies);
    }

    /// <summary>
    /// Calculates fundable amounts for a specific company.
    /// </summary>
    /// <param name="cik">The Central Index Key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated company with calculated amounts.</returns>
    /// <response code="200">Returns the company with calculated fundable amounts.</response>
    /// <response code="404">If the company is not found.</response>
    /// <response code="400">If the company doesn't have sufficient income data.</response>
    [HttpPost("calculate/{cik:int}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompanyDto>> CalculateFundableAmount(
        int cik,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating fundable amount for company with CIK: {Cik}", cik);
        var company = await _fundableAmountService.CalculateFundableAmountAsync(cik, cancellationToken);
        return Ok(company);
    }

    /// <summary>
    /// Calculates fundable amounts for all companies.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of companies processed.</returns>
    /// <response code="200">Returns the count of processed companies.</response>
    [HttpPost("calculate/all")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> CalculateAllFundableAmounts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating fundable amounts for all companies");
        var count = await _fundableAmountService.CalculateAllFundableAmountsAsync(cancellationToken);

        return Ok(new
        {
            message = "Fundable amounts calculated successfully",
            processedCount = count
        });
    }
}
