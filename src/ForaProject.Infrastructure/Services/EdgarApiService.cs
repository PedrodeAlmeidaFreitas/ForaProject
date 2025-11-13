using ForaProject.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForaProject.Infrastructure.Services;

/// <summary>
/// Implementation of SEC EDGAR API service.
/// </summary>
public class EdgarApiService : IEdgarApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<EdgarApiService> _logger;

    public EdgarApiService(HttpClient httpClient, ILogger<EdgarApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri("https://data.sec.gov/");
        
        // SEC EDGAR API requires User-Agent header as specified in challenge requirements
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<EdgarCompanyData?> GetCompanyDataAsync(int cik, CancellationToken cancellationToken = default)
    {
        try
        {
            // Format CIK with leading zeros (SEC EDGAR uses 10-digit CIKs)
            var formattedCik = cik.ToString("D10");
            
            // Fetch company submissions (contains company info)
            var submissionsUrl = $"submissions/CIK{formattedCik}.json";
            
            _logger.LogInformation("Fetching SEC data from: {Url}", $"{_httpClient.BaseAddress}{submissionsUrl}");
            
            var submissionsResponse = await _httpClient.GetAsync(submissionsUrl, cancellationToken);
            
            _logger.LogInformation("SEC API Response: StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}", 
                submissionsResponse.StatusCode, submissionsResponse.ReasonPhrase);
            
            if (!submissionsResponse.IsSuccessStatusCode)
            {
                var responseBody = await submissionsResponse.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("SEC API returned non-success status for CIK {Cik}. Status: {StatusCode}, Body: {Body}", 
                    cik, submissionsResponse.StatusCode, responseBody.Length > 500 ? responseBody[..500] : responseBody);
                return null;
            }

            var submissionsData = await submissionsResponse.Content.ReadFromJsonAsync<EdgarSubmissionsResponse>(_jsonOptions, cancellationToken);
            if (submissionsData == null)
            {
                return null;
            }

            var companyData = new EdgarCompanyData
            {
                Cik = cik,
                EntityName = submissionsData.Name ?? string.Empty,
                IncomeRecords = new List<Application.Interfaces.EdgarIncomeData>()
            };

            // Fetch company facts (contains financial data)
            var factsUrl = $"api/xbrl/companyfacts/CIK{formattedCik}.json";
            
            _logger.LogInformation("Fetching company facts from: {Url}", $"{_httpClient.BaseAddress}{factsUrl}");
            
            var factsResponse = await _httpClient.GetAsync(factsUrl, cancellationToken);
            
            _logger.LogInformation("Company Facts Response: StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}", 
                factsResponse.StatusCode, factsResponse.ReasonPhrase);
            
            if (factsResponse.IsSuccessStatusCode)
            {
                try
                {
                    var factsData = await factsResponse.Content.ReadFromJsonAsync<EdgarCompanyFactsResponse>(_jsonOptions, cancellationToken);
                    
                    _logger.LogInformation("Facts data parsed. UsGaap={HasUsGaap}, NetIncomeLoss={HasNetIncomeLoss}", 
                        factsData?.Facts?.UsGaap != null, 
                        factsData?.Facts?.UsGaap?.NetIncomeLoss != null);
                    
                    // Collect NetIncomeLoss data
                    if (factsData?.Facts?.UsGaap?.NetIncomeLoss?.Units?.Usd != null)
                    {
                        var incomeData = factsData.Facts.UsGaap.NetIncomeLoss.Units.Usd;
                        
                        _logger.LogInformation("Found {Count} NetIncomeLoss records for CIK {Cik}", 
                            incomeData?.Count ?? 0, cik);
                        
                        // Extract income data for years 2018-2022, 10-K filings only
                        // Filter out records with null fiscal year
                        var filteredIncomeData = incomeData!
                            .Where(r => r.Fy.HasValue && r.Fy >= 2018 && r.Fy <= 2022 && r.Form == "10-K")
                            .GroupBy(r => r.Fy!.Value)
                            .Select(g => g.OrderByDescending(r => r.Filed).First())
                            .ToList();

                        _logger.LogInformation("Filtered to {Count} income records (2018-2022, 10-K only) for CIK {Cik}", 
                            filteredIncomeData.Count, cik);
                        
                        foreach (var income in filteredIncomeData)
                        {
                            companyData.IncomeRecords.Add(new Application.Interfaces.EdgarIncomeData
                            {
                                Year = income.Fy!.Value,  // Safe to use .Value here because we filtered out nulls
                                Amount = income.Val,
                                Form = income.Form,
                                Frame = income.Frame,
                                FiledDate = income.Filed,
                                AccessionNumber = income.Accn
                            });
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No NetIncomeLoss data found for CIK {Cik}", cik);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse company facts for CIK {Cik}", cik);
                }
            }
            else
            {
                _logger.LogWarning("Company facts request failed for CIK {Cik}: {StatusCode}", 
                    cik, factsResponse.StatusCode);
            }

            return companyData;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request exception while fetching data for CIK {Cik}", cik);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing exception while fetching data for CIK {Cik}", cik);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception while fetching data for CIK {Cik}", cik);
            return null;
        }
    }
}

/// <summary>
/// Response model for SEC EDGAR submissions endpoint.
/// </summary>
internal class EdgarSubmissionsResponse
{
    public string Cik { get; set; } = string.Empty;
    public string? Name { get; set; }
}

/// <summary>
/// Response model for SEC EDGAR company facts endpoint.
/// </summary>
internal class EdgarCompanyFactsResponse
{
    public EdgarFacts? Facts { get; set; }
}

internal class EdgarFacts
{
    [JsonPropertyName("us-gaap")]
    public EdgarUsGaap? UsGaap { get; set; }
}

internal class EdgarUsGaap
{
    public EdgarNetIncomeLoss? NetIncomeLoss { get; set; }
}

internal class EdgarNetIncomeLoss
{
    public EdgarUnits? Units { get; set; }
}

internal class EdgarUnits
{
    [JsonPropertyName("USD")]
    public List<EdgarIncomeData>? Usd { get; set; }
}

internal class EdgarIncomeData
{
    public string Accn { get; set; } = string.Empty;
    public int? Fy { get; set; }  // Fiscal year can be null in some SEC filings
    public decimal Val { get; set; }
    public string Form { get; set; } = string.Empty;
    public string? Frame { get; set; }
    public DateTime Filed { get; set; }
}
