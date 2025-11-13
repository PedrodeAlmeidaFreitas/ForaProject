namespace ForaProject.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new company manually.
/// </summary>
public class CreateCompanyDto
{
    public int Cik { get; set; }
    public string EntityName { get; set; } = string.Empty;
}
