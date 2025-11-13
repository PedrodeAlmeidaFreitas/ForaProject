using AutoMapper;
using ForaProject.Application.DTOs;
using ForaProject.Application.Interfaces;
using ForaProject.Domain.Exceptions;
using ForaProject.Domain.Interfaces;
using ForaProject.Domain.ValueObjects;

namespace ForaProject.Application.Services;

/// <summary>
/// Implementation of fundable amount service for calculations and queries.
/// </summary>
public class FundableAmountService : IFundableAmountService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FundableAmountService(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FundableAmountDto>> GetFundableCompaniesAsync(CancellationToken cancellationToken = default)
    {
        var companies = await _companyRepository.GetEligibleForFundingAsync();
        var companiesList = companies.ToList();
        
        return companiesList.Select((c, index) => new FundableAmountDto
        {
            Id = index + 1,  // Sequential ID as per requirements
            Name = c.EntityName,
            StandardFundableAmount = c.StandardFundableAmount ?? 0,
            SpecialFundableAmount = c.SpecialFundableAmount ?? 0
        });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FundableAmountDto>> GetFundableCompaniesByLetterAsync(char startsWith, CancellationToken cancellationToken = default)
    {
        if (!char.IsLetter(startsWith))
        {
            throw new ArgumentException("Must be a letter.", nameof(startsWith));
        }

        var companies = await _companyRepository.GetEligibleForFundingAsync();
        var filteredCompanies = companies
            .Where(c => c.EntityName.StartsWith(startsWith.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        return filteredCompanies.Select((c, index) => new FundableAmountDto
        {
            Id = index + 1,  // Sequential ID as per requirements
            Name = c.EntityName,
            StandardFundableAmount = c.StandardFundableAmount ?? 0,
            SpecialFundableAmount = c.SpecialFundableAmount ?? 0
        });
    }

    /// <inheritdoc />
    public async Task<CompanyDto> CalculateFundableAmountAsync(int cik, CancellationToken cancellationToken = default)
    {
        var centralIndexKey = CentralIndexKey.Create(cik);
        var company = await _companyRepository.GetByCikWithIncomeRecordsAsync(centralIndexKey, cancellationToken);
        if (company == null)
        {
            throw new InvalidCompanyDataException($"Company with CIK {cik} not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Calculate fundable amounts using domain logic
            company.CalculateFundableAmounts();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return _mapper.Map<CompanyDto>(company);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> CalculateAllFundableAmountsAsync(CancellationToken cancellationToken = default)
    {
        var companies = await _companyRepository.GetAllWithIncomeRecordsAsync(cancellationToken);
        var processedCount = 0;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var company in companies)
            {
                try
                {
                    company.CalculateFundableAmounts();
                    processedCount++;
                }
                catch (InsufficientIncomeDataException)
                {
                    // Skip companies without sufficient income data
                    continue;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return processedCount;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
