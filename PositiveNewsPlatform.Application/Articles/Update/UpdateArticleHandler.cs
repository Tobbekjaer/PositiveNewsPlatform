using Microsoft.Extensions.Logging;
using PositiveNewsPlatform.Application.Abstractions.Persistence;
using PositiveNewsPlatform.Application.Abstractions.ReadModel;
using PositiveNewsPlatform.Application.Articles.GetById;
using PositiveNewsPlatform.Domain.Articles;

namespace PositiveNewsPlatform.Application.Articles.Update;

public sealed class UpdateArticleHandler
{
    private readonly IArticleWriteRepository _articles;
    private readonly IUnitOfWork _uow;

    // read-side sync
    private readonly IArticleReadRepository _readRepo;
    private readonly IArticleCache _cache;

    private readonly ILogger<UpdateArticleHandler> _logger;

    public UpdateArticleHandler(
        IArticleWriteRepository articles,
        IUnitOfWork uow,
        IArticleReadRepository readRepo,
        IArticleCache cache,
        ILogger<UpdateArticleHandler> logger)
    {
        _articles = articles;
        _uow = uow;
        _readRepo = readRepo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UpdateArticleResult> HandleAsync(UpdateArticleCommand cmd, CancellationToken ct)
    {
        _logger.LogInformation("UpdateArticle started. ArticleId={ArticleId}", cmd.ArticleId);

        var article = await _articles.GetByIdAsync(new ArticleId(cmd.ArticleId), ct);
        if (article is null)
        {
            _logger.LogInformation("UpdateArticle not found. ArticleId={ArticleId}", cmd.ArticleId);
            return new UpdateArticleResult(Updated: false);
        }

        await _uow.BeginTransactionAsync(ct);

        try
        {
            article.Update(cmd.Title, cmd.Content);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("UpdateArticle committed to SQL. ArticleId={ArticleId}", cmd.ArticleId);
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync(ct);
            _logger.LogWarning(ex, "UpdateArticle rolled back. ArticleId={ArticleId}", cmd.ArticleId);
            throw;
        }

        // Eventual consistency: invalidate + upsert read model
        _logger.LogInformation("UpdateArticle syncing read model (eventual consistency). ArticleId={ArticleId}", cmd.ArticleId);

        await _cache.RemoveArticleAsync(cmd.ArticleId, ct);
        await _cache.RemoveLatestAsync(ct);

        await _readRepo.UpsertAsync(new ArticleDto(
            ArticleId: article.Id.Value,
            Title: article.Title,
            Content: article.Content,
            Status: article.Status.ToString(),
            CreatedAtUtc: article.CreatedAt,
            UpdatedAtUtc: article.UpdatedAt,
            Media: Array.Empty<MediaDto>()
        ), ct);

        return new UpdateArticleResult(Updated: true);
    }
}
