using Microsoft.Extensions.Logging;
using PositiveNewsPlatform.Application.Abstractions.Persistence;
using PositiveNewsPlatform.Application.Abstractions.ReadModel;
using PositiveNewsPlatform.Application.Abstractions.Storage;
using PositiveNewsPlatform.Domain.Articles;

namespace PositiveNewsPlatform.Application.Articles.Create;

public sealed class CreateArticleHandler
{
    private readonly IArticleWriteRepository _articles;
    private readonly IMediaRegistryWriteRepository _mediaRegistry;
    private readonly IUnitOfWork _uow;
    private readonly IObjectStorage _objectStorage;

    // read-side sync (eventual consistency)
    private readonly IArticleReadRepository _readRepo;
    private readonly IArticleCache _cache;

    private readonly ILogger<CreateArticleHandler> _logger;

    public CreateArticleHandler(
        IArticleWriteRepository articles,
        IMediaRegistryWriteRepository mediaRegistry,
        IUnitOfWork uow,
        IObjectStorage objectStorage,
        IArticleReadRepository readRepo,
        IArticleCache cache,
        ILogger<CreateArticleHandler> logger)
    {
        _articles = articles;
        _mediaRegistry = mediaRegistry;
        _uow = uow;
        _objectStorage = objectStorage;
        _readRepo = readRepo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CreateArticleResult> HandleAsync(CreateArticleCommand cmd, CancellationToken ct)
    {
        _logger.LogInformation("CreateArticle started. HasImage={HasImage}", cmd.Image is not null);

        var article = Article.Create(cmd.Title, cmd.Content);
        
        var mediaList = new List<GetById.MediaDto>();

        await _uow.BeginTransactionAsync(ct);

        try
        {
            await _articles.AddAsync(article, ct);
            _logger.LogInformation("CreateArticle staged in SQL context. ArticleId={ArticleId}", article.Id.Value);

            if (cmd.Image is not null)
            {
                _logger.LogInformation("CreateArticle uploading image to object storage. ArticleId={ArticleId}", article.Id.Value);

                var stored = await _objectStorage.UploadAsync(
                    cmd.Image.Content,
                    cmd.Image.SizeBytes,
                    cmd.Image.FileName,
                    cmd.Image.ContentType,
                    ct);

                _logger.LogInformation(
                    "CreateArticle image uploaded. ArticleId={ArticleId}, ObjectKey={ObjectKey}, SizeBytes={SizeBytes}",
                    article.Id.Value, stored.ObjectKey, stored.SizeBytes);

                // Write-side registry in SQL
                var mediaRow = new MediaRegistryRow(
                    MediaId: Guid.NewGuid(),
                    ArticleId: article.Id.Value,
                    ObjectKey: stored.ObjectKey,
                    MimeType: stored.ContentType,
                    SizeBytes: stored.SizeBytes,
                    UploadedAtUtc: DateTime.UtcNow);

                await _mediaRegistry.AddAsync(mediaRow, ct);

                _logger.LogInformation(
                    "CreateArticle media metadata staged in SQL context. ArticleId={ArticleId}, MediaId={MediaId}",
                    article.Id.Value, mediaRow.MediaId);
                
                // Read-side projection data (denormalized)
                mediaList.Add(new GetById.MediaDto(
                    ObjectKey: stored.ObjectKey,
                    MimeType: stored.ContentType,
                    SizeBytes: stored.SizeBytes
                ));
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("CreateArticle committed to SQL. ArticleId={ArticleId}", article.Id.Value);
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync(ct);
            _logger.LogWarning(ex, "CreateArticle rolled back. ArticleId={ArticleId}", article.Id.Value);
            throw;
        }

        // Eventual consistency: update read model + invalidate cache
        _logger.LogInformation("CreateArticle syncing read model (eventual consistency). ArticleId={ArticleId}", article.Id.Value);

        await _cache.RemoveArticleAsync(article.Id.Value, ct);
        await _cache.RemoveLatestAsync(ct);

        await _readRepo.UpsertAsync(new GetById.ArticleDto(
            ArticleId: article.Id.Value,
            Title: article.Title,
            Content: article.Content,
            Status: article.Status.ToString(),
            CreatedAtUtc: article.CreatedAt,
            UpdatedAtUtc: article.UpdatedAt,
            Media: Array.Empty<GetById.MediaDto>()
        ), ct);

        _logger.LogInformation("CreateArticle read model updated. ArticleId={ArticleId}", article.Id.Value);

        return new CreateArticleResult(article.Id.Value);
    }
}
