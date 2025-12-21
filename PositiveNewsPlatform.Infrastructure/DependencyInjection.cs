using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PositiveNewsPlatform.Application.Abstractions.Persistence;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql.Repositories;

namespace PositiveNewsPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("Sql");

        services.AddDbContext<PositiveNewsDbContext>(opt =>
            opt.UseSqlServer(connStr));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IArticleWriteRepository, ArticleWriteRepository>();
        services.AddScoped<IMediaRegistryWriteRepository, MediaRegistryWriteRepository>();

        return services;
    }
}