namespace ForaProject.Application.DTOs;

/// <summary>
/// Data transfer object for company information.
/// </summary>
public class CompanyDto
{
    public Guid Id { get; set; }
    public int Cik { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public decimal? StandardFundableAmount { get; set; }
    public decimal? SpecialFundableAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<IncomeRecordDto> IncomeRecords { get; set; } = new();
}
