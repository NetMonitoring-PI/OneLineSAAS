using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using OneLine.Auth.Domain.Entities;
using OneLine.Auth.Domain.Interfaces;

namespace OneLine.Auth.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthDbContext _context;

    public RefreshTokenRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token, ct);
    }

    public async Task AddAsync(
        RefreshToken token,
        CancellationToken ct = default)
    {
        await _context.RefreshTokens.AddAsync(token, ct);
    }

    public void Update(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
    }

    public async Task DeleteExpiredTokensAsync(
        CancellationToken ct = default)
    {
        // Nettoyage périodique des tokens expirés
        // Appelé par un background job (Hangfire plus tard)
        var expiredTokens = await _context.RefreshTokens
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

        _context.RefreshTokens.RemoveRange(expiredTokens);
    }
}