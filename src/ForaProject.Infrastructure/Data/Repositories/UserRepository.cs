using Microsoft.EntityFrameworkCore;
using ForaProject.Domain.Entities;
using ForaProject.Domain.Interfaces;
using ForaProject.Infrastructure.Data.Contexts;

namespace ForaProject.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for User aggregate operations.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.Email == email.ToLowerInvariant());
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _context.Set<User>()
            .Where(u => u.IsActive && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        // Note: EF Core doesn't support querying collections directly in this way
        // We'll load all users and filter in memory (for small datasets this is acceptable)
        // For large datasets, consider storing roles in a separate table
        var allUsers = await _context.Set<User>()
            .Where(u => !u.IsDeleted)
            .ToListAsync();

        return allUsers.Where(u => u.HasRole(role));
    }
}
