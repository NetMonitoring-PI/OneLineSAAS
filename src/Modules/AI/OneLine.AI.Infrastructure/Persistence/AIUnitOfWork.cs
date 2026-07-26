using OneLine.AI.Application.Interfaces;

namespace OneLine.AI.Infrastructure.Persistence;

public sealed class AIUnitOfWork : IUnitOfWork
{
    private readonly AIDbContext _context;
    public AIUnitOfWork(AIDbContext context) => _context = context;
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
