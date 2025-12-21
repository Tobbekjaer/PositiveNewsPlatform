using Microsoft.EntityFrameworkCore;
using PositiveNewsPlatform.Domain.Articles;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql.Models;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql;

public sealed class PositiveNewsDbContext : DbContext
{
    public PositiveNewsDbContext(DbContextOptions<PositiveNewsDbContext> options)
        : base(options) { }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<MediaRegistryWriteModel> MediaRegistry => Set<MediaRegistryWriteModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PositiveNewsDbContext).Assembly);
    }
}