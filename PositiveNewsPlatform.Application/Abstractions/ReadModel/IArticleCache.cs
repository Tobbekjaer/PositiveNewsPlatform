using PositiveNewsPlatform.Application.Articles.GetById;

namespace PositiveNewsPlatform.Application.Abstractions.ReadModel;

public interface IArticleCache
{
    Task<ArticleDto?> GetArticleAsync(Guid articleId, CancellationToken ct);
    Task SetArticleAsync(Guid articleId, ArticleDto dto, TimeSpan ttl, CancellationToken ct);
    Task RemoveArticleAsync(Guid articleId, CancellationToken ct);
    
    Task<IReadOnlyList<ArticleDto>> GetLatestAsync(int take, CancellationToken ct);
    Task SetLatestAsync(int take, IReadOnlyList<ArticleDto> dtos, TimeSpan ttl, CancellationToken ct);
    Task RemoveLatestAsync(CancellationToken ct);
}