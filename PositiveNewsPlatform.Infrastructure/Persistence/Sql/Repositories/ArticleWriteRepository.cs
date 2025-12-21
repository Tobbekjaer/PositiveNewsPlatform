using Microsoft.EntityFrameworkCore;
using PositiveNewsPlatform.Application.Abstractions.Persistence;
using PositiveNewsPlatform.Domain.Articles;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql.Repositories;

public sealed class ArticleWriteRepository : IArticleWriteRepository
{
    private readonly PositiveNewsDbContext _db;

    public ArticleWriteRepository(PositiveNewsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Article article, CancellationToken ct)
    {
        await _db.Articles.AddAsync(article, ct);
    }

    public Task<Article?> GetByIdAsync(ArticleId id, CancellationToken ct)
    {
        return _db.Articles.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}