using PositiveNewsPlatform.Domain.Articles;

namespace PositiveNewsPlatform.Application.Abstractions.Persistence;

public interface IArticleWriteRepository
{
    Task AddAsync(Article article, CancellationToken ct);
    Task<Article?> GetByIdAsync(ArticleId id, CancellationToken ct);
}