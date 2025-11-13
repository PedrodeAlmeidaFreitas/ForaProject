namespace ForaProject.Application.DTOs;

/// <summary>
/// Data transfer object for batch importing companies from SEC EDGAR.
/// </summary>
public class BatchImportDto
{
    public List<int> Ciks { get; set; } = new();
}
