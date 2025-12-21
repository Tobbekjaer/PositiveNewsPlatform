using MongoDB.Driver;
using PositiveNewsPlatform.Application.Abstractions.ReadModel;
using PositiveNewsPlatform.Application.Articles.GetById;
using PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Mongo.Documents;

namespace PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Mongo;

public sealed class MongoArticleReadRepository : IArticleReadRepository
{
    private readonly IMongoCollection<ArticleDocument> _articles;

    public MongoArticleReadRepository(IMongoDatabase db)
    {
        _articles = db.GetCollection<ArticleDocument>(MongoCollections.Articles);
    }

    public async Task<ArticleDto?> GetByIdAsync(Guid articleId, CancellationToken ct)
    {
        var doc = await _articles
            .Find(x => x.ArticleId == articleId)
            .FirstOrDefaultAsync(ct);

        return doc is null ? null : MapToDto(doc);
    }

    public async Task<IReadOnlyList<ArticleDto>> GetLatestAsync(int take, CancellationToken ct)
    {
        if (take <= 0) take = 10;

        var docs = await _articles
            .Find(FilterDefinition<ArticleDocument>.Empty)
            .SortByDescending(x => x.CreatedAtUtc)
            .Limit(take)
            .ToListAsync(ct);

        return docs.Select(MapToDto).ToList();
    }

    public async Task UpsertAsync(ArticleDto article, CancellationToken ct)
    {
        var doc = MapToDoc(article);

        await _articles.ReplaceOneAsync(
            filter: x => x.ArticleId == article.ArticleId,
            replacement: doc,
            options: new ReplaceOptions { IsUpsert = true },
            cancellationToken: ct);
    }

    private static ArticleDto MapToDto(ArticleDocument doc)
        => new(
            ArticleId: doc.ArticleId,
            Title: doc.Title,
            Content: doc.Content,
            Status: doc.Status,
            CreatedAtUtc: doc.CreatedAtUtc,
            UpdatedAtUtc: doc.UpdatedAtUtc,
            Media: Array.Empty<MediaDto>() // Media kommer senere
        );

    private static ArticleDocument MapToDoc(ArticleDto dto)
        => new()
        {
            ArticleId = dto.ArticleId,
            Title = dto.Title,
            Content = dto.Content,
            Status = dto.Status,
            CreatedAtUtc = dto.CreatedAtUtc,
            UpdatedAtUtc = dto.UpdatedAtUtc
        };
}
