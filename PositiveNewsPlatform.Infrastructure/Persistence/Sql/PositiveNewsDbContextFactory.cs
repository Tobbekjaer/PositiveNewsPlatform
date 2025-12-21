using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql;

public sealed class PositiveNewsDbContextFactory : IDesignTimeDbContextFactory<PositiveNewsDbContext>
{
    public PositiveNewsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PositiveNewsDbContext>();

        // Design-time fallback connection string (local dev)
        optionsBuilder.UseSqlServer(
            "Server=localhost,14340;Database=PositiveNewsDb;User Id=sa;Password=Villamaj201$;TrustServerCertificate=True;Encrypt=True");

        return new PositiveNewsDbContext(optionsBuilder.Options);
    }
}