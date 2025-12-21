using PositiveNewsPlatform.Application.Articles.GetById;

namespace PositiveNewsPlatform.Application.Abstractions.ReadModel;

public interface IArticleReadRepository
{
    Task<ArticleDto?> GetByIdAsync(Guid articleId, CancellationToken ct);
    Task<IReadOnlyList<ArticleDto>> GetLatestAsync(int take, CancellationToken ct);
    Task UpsertAsync(ArticleDto article, CancellationToken ct);
}