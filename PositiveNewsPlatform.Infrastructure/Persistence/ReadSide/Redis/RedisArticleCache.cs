using System.Text.Json;
using PositiveNewsPlatform.Application.Abstractions.ReadModel;
using PositiveNewsPlatform.Application.Articles.GetById;
using StackExchange.Redis;

namespace PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Redis;

public sealed class RedisArticleCache : IArticleCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDatabase _db;

    public RedisArticleCache(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public async Task<ArticleDto?> GetArticleAsync(Guid articleId, CancellationToken ct)
    {
        var value = await _db.StringGetAsync(RedisKeys.Article(articleId));
        if (!value.HasValue) return null;

        return JsonSerializer.Deserialize<ArticleDto>(value!, JsonOptions);
    }

    public async Task SetArticleAsync(Guid articleId, ArticleDto dto, TimeSpan ttl, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(dto, JsonOptions);
        await _db.StringSetAsync(RedisKeys.Article(articleId), json, ttl);
    }

    public async Task RemoveArticleAsync(Guid articleId, CancellationToken ct)
    {
        await _db.KeyDeleteAsync(RedisKeys.Article(articleId));
    }

    public async Task<IReadOnlyList<ArticleDto>> GetLatestAsync(int take, CancellationToken ct)
    {
        var value = await _db.StringGetAsync(RedisKeys.Latest(take));
        if (!value.HasValue) return Array.Empty<ArticleDto>();

        var list = JsonSerializer.Deserialize<List<ArticleDto>>(value!, JsonOptions);
        return list is null ? Array.Empty<ArticleDto>() : list;
    }

    public async Task SetLatestAsync(int take, IReadOnlyList<ArticleDto> dtos, TimeSpan ttl, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(dtos, JsonOptions);
        await _db.StringSetAsync(RedisKeys.Latest(take), json, ttl);
    }

    public async Task RemoveLatestAsync(CancellationToken ct)
    {
        // Vi sletter ikke alle mulige takes - kun dem vi reelt bruger.
        // Hold det simpelt: slet latest:5 som default.
        await _db.KeyDeleteAsync(RedisKeys.Latest(5));
    }
}
