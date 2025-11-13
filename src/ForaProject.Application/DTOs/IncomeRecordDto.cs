namespace ForaProject.Application.DTOs;

/// <summary>
/// Data transfer object for income record information.
/// </summary>
public class IncomeRecordDto
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public decimal Amount { get; set; }
    public string Form { get; set; } = string.Empty;
    public string? Frame { get; set; }
    public DateTime FiledDate { get; set; }
    public string AccessionNumber { get; set; } = string.Empty;
}
