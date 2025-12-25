using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using MongoDB.Driver;
using PositiveNewsPlatform.Application.Abstractions.Persistence;
using PositiveNewsPlatform.Application.Abstractions.ReadModel;
using PositiveNewsPlatform.Application.Abstractions.Storage;
using PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Mongo;
using PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Redis;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql.Repositories;
using PositiveNewsPlatform.Infrastructure.Storage.Minio;
using StackExchange.Redis;

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

        // Add Mongo
        services.AddMongo(config);
        // Add Redis
        services.AddRedis(config);
        // Add Minio
        services.AddMinio(config);
        
        return services;
    }

    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration config)
    {
        // Mongo
        services.Configure<MongoOptions>(o =>
            config.GetSection(MongoOptions.SectionName).Bind(o));
        
        services.AddSingleton<IMongoClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            var url = MongoUrl.Create(opts.ConnectionString);

            var settings = MongoClientSettings.FromUrl(url);

            // Read preference controls where read queries are served from
            // (e.g. primary vs secondaries for availability)
            settings.ReadPreference = opts.ReadPreference switch
            {
                "SecondaryPreferred" => ReadPreference.SecondaryPreferred,
                "Nearest"            => ReadPreference.Nearest,
                _                    => ReadPreference.Primary
            };

            // Write concern controls durability guarantees for writes
            // (e.g. majority vs single-node acknowledgement)
            settings.WriteConcern = opts.WriteConcern switch
            {
                "W1" => WriteConcern.W1,
                _    => WriteConcern.WMajority
            };

            return new MongoClient(settings);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(opts.Database);
        });

        services.AddScoped<IArticleReadRepository, MongoArticleReadRepository>();

        return services;
    }
    
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
    {
        // Redis
        services.Configure<RedisOptions>(o =>
            config.GetSection(RedisOptions.SectionName).Bind(o));
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(opts.ConnectionString);
        });

        services.AddScoped<IArticleCache, RedisArticleCache>();

        return services;
    }
    
    public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration config)
    {
        // Minio
        services.Configure<MinioOptions>(o => config.GetSection(MinioOptions.SectionName).Bind(o));
        
        services.AddSingleton<IMinioClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<MinioOptions>>().Value;

            return new MinioClient()
                .WithEndpoint(opts.Endpoint)
                .WithCredentials(opts.AccessKey, opts.SecretKey)
                .WithSSL(opts.UseSsl)
                .Build();
        });
        
        services.AddSingleton<IObjectStorage, MinioObjectStorage>();
        
        return services;
    }
}