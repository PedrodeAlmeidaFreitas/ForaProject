using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Interfaces;
using ForaProject.Domain.ValueObjects;
using ForaProject.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ForaProject.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Company aggregate.
/// </summary>
public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<Company?> GetByCikAsync(CentralIndexKey cik, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Cik.Value == cik.Value, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Company?> GetByCikWithIncomeRecordsAsync(CentralIndexKey cik, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.IncomeRecords)
            .FirstOrDefaultAsync(c => c.Cik.Value == cik.Value, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Company>> GetEligibleForFundingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.StandardFundableAmount != null && c.StandardFundableAmount > 0)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Company>> GetByNameStartsWithAsync(char letter, CancellationToken cancellationToken = default)
    {
        var upperLetter = char.ToUpperInvariant(letter);
        return await _dbSet
            .Where(c => c.StandardFundableAmount != null &&
                        c.StandardFundableAmount > 0 &&
                        c.EntityName.StartsWith(upperLetter.ToString()))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(CentralIndexKey cik, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(c => c.Cik.Value == cik.Value, cancellationToken);
    }

    /// <summary>
    /// Gets a company by ID including income records.
    /// </summary>
    /// <param name="id">The company ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The company with income records if found; otherwise, null.</returns>
    public async Task<Company?> GetByIdWithIncomeRecordsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.IncomeRecords)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets all companies including income records.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All companies with income records.</returns>
    public async Task<IEnumerable<Company>> GetAllWithIncomeRecordsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.IncomeRecords)
            .ToListAsync(cancellationToken);
    }
}
