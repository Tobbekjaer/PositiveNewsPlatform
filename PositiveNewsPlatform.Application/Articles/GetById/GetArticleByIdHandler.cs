using Microsoft.Extensions.Logging;
using PositiveNewsPlatform.Application.Abstractions.ReadModel;

namespace PositiveNewsPlatform.Application.Articles.GetById;

public sealed class GetArticleByIdHandler
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly IArticleReadRepository _readRepo;
    private readonly IArticleCache _cache;
    private readonly ILogger<GetArticleByIdHandler> _logger;

    public GetArticleByIdHandler(
        IArticleReadRepository readRepo,
        IArticleCache cache,
        ILogger<GetArticleByIdHandler> logger)
    {
        _readRepo = readRepo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ArticleDto?> HandleAsync(GetArticleByIdQuery query, CancellationToken ct)
    {
        _logger.LogInformation("GetArticleById started. ArticleId={ArticleId}", query.ArticleId);

        var cached = await _cache.GetArticleAsync(query.ArticleId, ct);
        if (cached is not null)
        {
            _logger.LogInformation("GetArticleById cache hit. ArticleId={ArticleId}", query.ArticleId);
            return cached;
        }

        _logger.LogInformation("GetArticleById cache miss. Reading from MongoDB. ArticleId={ArticleId}", query.ArticleId);

        var fromReadModel = await _readRepo.GetByIdAsync(query.ArticleId, ct);
        if (fromReadModel is null)
        {
            _logger.LogInformation("GetArticleById not found in read model. ArticleId={ArticleId}", query.ArticleId);
            return null;
        }

        await _cache.SetArticleAsync(query.ArticleId, fromReadModel, CacheTtl, ct);

        _logger.LogInformation("GetArticleById cached result. ArticleId={ArticleId}", query.ArticleId);
        return fromReadModel;
    }
}