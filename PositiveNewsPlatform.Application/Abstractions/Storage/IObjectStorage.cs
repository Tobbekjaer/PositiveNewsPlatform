namespace PositiveNewsPlatform.Application.Abstractions.Storage;

public sealed record StoredObject(
    string ObjectKey,
    string ContentType,
    long SizeBytes
);

public interface IObjectStorage
{
    Task<StoredObject> UploadAsync(
        Stream content,
        long sizeBytes,
        string fileName,
        string contentType,
        CancellationToken ct);
}