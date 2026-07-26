using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OneLine.AI.Infrastructure.Persistence;

public sealed class AIDbContextFactory
    : IDesignTimeDbContextFactory<AIDbContext>
{
    public AIDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AIDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=oneline_saaskit;Username=postgres;Password=postgres");
        return new AIDbContext(optionsBuilder.Options);
    }
}
