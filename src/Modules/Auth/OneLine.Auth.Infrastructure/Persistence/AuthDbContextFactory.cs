using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OneLine.Auth.Infrastructure.Persistence;

public sealed class AuthDbContextFactory
    : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=oneline_saaskit;Username=postgres;Password=postgres");
        return new AuthDbContext(optionsBuilder.Options);
    }
}
