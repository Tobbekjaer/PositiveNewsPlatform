using PositiveNewsPlatform.Application.Abstractions.Persistence;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql.Models;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql.Repositories;

public sealed class MediaRegistryWriteRepository : IMediaRegistryWriteRepository
{
    private readonly PositiveNewsDbContext _db;

    public MediaRegistryWriteRepository(PositiveNewsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(MediaRegistryRow row, CancellationToken ct)
    {
        var model = new MediaRegistryWriteModel
        {
            MediaId = row.MediaId,
            ArticleId = row.ArticleId,
            ObjectKey = row.ObjectKey,
            MimeType = row.MimeType,
            SizeBytes = row.SizeBytes,
            UploadedAtUtc = row.UploadedAtUtc
        };

        await _db.MediaRegistry.AddAsync(model, ct);
    }
}