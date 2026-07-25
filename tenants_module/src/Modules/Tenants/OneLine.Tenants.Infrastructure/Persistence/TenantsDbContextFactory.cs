using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OneLine.Tenants.Infrastructure.Persistence;

public sealed class TenantsDbContextFactory
    : IDesignTimeDbContextFactory<TenantsDbContext>
{
    public TenantsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantsDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=oneline_saaskit;Username=postgres;Password=postgres");
        return new TenantsDbContext(optionsBuilder.Options);
    }
}
