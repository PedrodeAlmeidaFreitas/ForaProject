using AutoMapper;
using ForaProject.Application.DTOs;
using ForaProject.Application.Interfaces;
using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Entities;
using ForaProject.Domain.Exceptions;
using ForaProject.Domain.Interfaces;
using ForaProject.Domain.ValueObjects;

namespace ForaProject.Application.Services;

/// <summary>
/// Implementation of company service for managing company operations.
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IEdgarApiService _edgarApiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompanyService(
        ICompanyRepository companyRepository,
        IEdgarApiService edgarApiService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _edgarApiService = edgarApiService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(CancellationToken cancellationToken = default)
    {
        var companies = await _companyRepository.GetAllWithIncomeRecordsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    /// <inheritdoc />
    public async Task<CompanyDto?> GetCompanyByCikAsync(int cik, CancellationToken cancellationToken = default)
    {
        var centralIndexKey = CentralIndexKey.Create(cik);
        var company = await _companyRepository.GetByCikWithIncomeRecordsAsync(centralIndexKey, cancellationToken);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    /// <inheritdoc />
    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    /// <inheritdoc />
    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto, CancellationToken cancellationToken = default)
    {
        // Check if company already exists
        var centralIndexKey = CentralIndexKey.Create(createCompanyDto.Cik);
        var existingCompany = await _companyRepository.GetByCikAsync(centralIndexKey, cancellationToken);
        if (existingCompany != null)
        {
            throw new InvalidCompanyDataException($"Company with CIK {createCompanyDto.Cik} already exists.");
        }

        var cik = CentralIndexKey.Create(createCompanyDto.Cik);
        var company = Company.Create(cik, createCompanyDto.EntityName);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _companyRepository.AddAsync(company);
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
    public async Task<CompanyDto> ImportCompanyAsync(ImportCompanyDto importDto, CancellationToken cancellationToken = default)
    {
        // Check if company already exists
        var centralIndexKey = CentralIndexKey.Create(importDto.Cik);
        var existingCompany = await _companyRepository.GetByCikAsync(centralIndexKey, cancellationToken);
        if (existingCompany != null)
        {
            throw new InvalidCompanyDataException($"Company with CIK {importDto.Cik} already exists.");
        }

        // Fetch data from SEC EDGAR API
        var edgarData = await _edgarApiService.GetCompanyDataAsync(importDto.Cik, cancellationToken);
        if (edgarData == null)
        {
            throw new InvalidCompanyDataException($"Company with CIK {importDto.Cik} not found in SEC EDGAR database.");
        }

        var cik = CentralIndexKey.Create(edgarData.Cik);
        var company = Company.Create(cik, edgarData.EntityName);

        // Add income records from EDGAR data
        foreach (var incomeData in edgarData.IncomeRecords)
        {
            var incomeRecord = IncomeRecord.Create(
                company.Id,
                incomeData.Year,
                incomeData.Amount,
                incomeData.Form,
                incomeData.Frame,
                incomeData.FiledDate,
                incomeData.AccessionNumber
            );
            company.AddIncomeRecord(incomeRecord);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _companyRepository.AddAsync(company);
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
    public async Task<IEnumerable<CompanyDto>> BatchImportCompaniesAsync(BatchImportDto batchImportDto, CancellationToken cancellationToken = default)
    {
        var importedCompanies = new List<CompanyDto>();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var cik in batchImportDto.Ciks)
            {
                // Check if company already exists
                var centralIndexKey = CentralIndexKey.Create(cik);
                var existingCompany = await _companyRepository.GetByCikAsync(centralIndexKey, cancellationToken);
                if (existingCompany != null)
                {
                    continue; // Skip existing companies in batch import
                }

                // Fetch data from SEC EDGAR API
                var edgarData = await _edgarApiService.GetCompanyDataAsync(cik, cancellationToken);
                if (edgarData == null)
                {
                    continue; // Skip if not found
                }

                var cikValue = CentralIndexKey.Create(edgarData.Cik);
                var company = Company.Create(cikValue, edgarData.EntityName);

                // Add income records from EDGAR data
                foreach (var incomeData in edgarData.IncomeRecords)
                {
                    var incomeRecord = IncomeRecord.Create(
                        company.Id,
                        incomeData.Year,
                        incomeData.Amount,
                        incomeData.Form,
                        incomeData.Frame,
                        incomeData.FiledDate,
                        incomeData.AccessionNumber
                    );
                    company.AddIncomeRecord(incomeRecord);
                }

                await _companyRepository.AddAsync(company);
                importedCompanies.Add(_mapper.Map<CompanyDto>(company));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return importedCompanies;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteCompanyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null)
        {
            throw new InvalidCompanyDataException($"Company with ID {id} not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _companyRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
