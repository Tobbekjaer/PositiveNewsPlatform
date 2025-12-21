using Microsoft.Extensions.Logging;
using PositiveNewsPlatform.Application.Abstractions.ReadModel;
using PositiveNewsPlatform.Application.Articles.GetById;

namespace PositiveNewsPlatform.Application.Articles.GetLatest;

public sealed class GetLatestArticlesHandler
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

    private readonly IArticleReadRepository _readRepo;
    private readonly IArticleCache _cache;
    private readonly ILogger<GetLatestArticlesHandler> _logger;

    public GetLatestArticlesHandler(
        IArticleReadRepository readRepo,
        IArticleCache cache,
        ILogger<GetLatestArticlesHandler> logger)
    {
        _readRepo = readRepo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ArticleDto>> HandleAsync(GetLatestArticlesQuery query, CancellationToken ct)
    {
        var take = query.Take <= 0 ? 10 : query.Take;

        _logger.LogInformation("GetLatestArticles started. Take={Take}", take);

        var cached = await _cache.GetLatestAsync(take, ct);
        if (cached.Count > 0)
        {
            _logger.LogInformation("GetLatestArticles cache hit. Take={Take}, Count={Count}", take, cached.Count);
            return cached;
        }

        _logger.LogInformation("GetLatestArticles cache miss. Reading from MongoDB. Take={Take}", take);

        var latest = await _readRepo.GetLatestAsync(take, ct);

        _logger.LogInformation("GetLatestArticles read from MongoDB. Take={Take}, Count={Count}", take, latest.Count);

        await _cache.SetLatestAsync(take, latest, CacheTtl, ct);
        return latest;
    }
}