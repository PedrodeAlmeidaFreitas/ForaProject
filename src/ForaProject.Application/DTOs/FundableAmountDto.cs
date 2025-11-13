namespace ForaProject.Application.DTOs;

/// <summary>
/// Data transfer object for fundable amount information.
/// Response format as specified in challenge requirements.
/// </summary>
public class FundableAmountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal StandardFundableAmount { get; set; }
    public decimal SpecialFundableAmount { get; set; }
}
