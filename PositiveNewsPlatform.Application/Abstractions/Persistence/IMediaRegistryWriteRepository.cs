namespace PositiveNewsPlatform.Application.Abstractions.Persistence;

public sealed record MediaRegistryRow(
    Guid MediaId,
    Guid ArticleId,
    string ObjectKey,
    string MimeType,
    long SizeBytes,
    DateTime UploadedAtUtc
);

public interface IMediaRegistryWriteRepository
{
    Task AddAsync(MediaRegistryRow row, CancellationToken ct);
}