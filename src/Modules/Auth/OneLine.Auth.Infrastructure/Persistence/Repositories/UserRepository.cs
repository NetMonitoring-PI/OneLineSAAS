using Microsoft.EntityFrameworkCore;
using OneLine.Auth.Domain.Entities;
using OneLine.Auth.Domain.Interfaces;

namespace OneLine.Auth.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<AppUser?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        return await _context.Set<AppUser>()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<AppUser?> GetByEmailAsync(
        string email, CancellationToken ct = default)
    {
        return await _context.Set<AppUser>()
            .FirstOrDefaultAsync(
                u => u.NormalizedEmail == email.ToUpperInvariant(), ct);
    }

    public async Task<AppUser?> GetByIdWithTokensAsync(
        Guid id, CancellationToken ct = default)
    {
        return await _context.Set<AppUser>()
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email, CancellationToken ct = default)
    {
        return await _context.Set<AppUser>()
            .AnyAsync(
                u => u.NormalizedEmail == email.ToUpperInvariant(), ct);
    }

    public async Task AddAsync(
        AppUser user, CancellationToken ct = default)
    {
        await _context.Set<AppUser>().AddAsync(user, ct);
    }

    public void Update(AppUser user)
    {
        _context.Set<AppUser>().Update(user);
    }

    public void Delete(AppUser user)
    {
        _context.Set<AppUser>().Remove(user);
    }
}
