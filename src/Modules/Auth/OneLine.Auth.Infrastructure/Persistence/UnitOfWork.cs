using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneLine.Auth.Application.Interfaces;

namespace OneLine.Auth.Infrastructure.Persistence;

/// <summary>
/// Implémentation du Unit of Work.
/// Délègue simplement à DbContext.SaveChangesAsync().
///
/// Toutes les opérations depuis le début de la requête
/// sont sauvegardées en une seule transaction.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;

    public UnitOfWork(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}