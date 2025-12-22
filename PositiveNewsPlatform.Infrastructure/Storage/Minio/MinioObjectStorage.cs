using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using PositiveNewsPlatform.Application.Abstractions.Storage;

namespace PositiveNewsPlatform.Infrastructure.Storage.Minio;

public sealed class MinioObjectStorage : IObjectStorage
{
    private readonly IMinioClient _client;
    private readonly MinioOptions _options;

    public MinioObjectStorage(IMinioClient client, IOptions<MinioOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<StoredObject> UploadAsync(
        Stream content,
        long sizeBytes,
        string fileName,
        string contentType,
        CancellationToken ct)
    {
        await EnsureBucketExistsAsync(ct);

        var objectKey = $"{Guid.NewGuid()}-{SanitizeFileName(fileName)}";

        var putArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithContentType(contentType)
            .WithStreamData(content)
            .WithObjectSize(sizeBytes);

        await _client.PutObjectAsync(putArgs, ct);

        return new StoredObject(
            ObjectKey: objectKey,
            ContentType: contentType,
            SizeBytes: sizeBytes
        );
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        var exists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.Bucket),
            ct);

        if (exists) return;

        await _client.MakeBucketAsync(
            new MakeBucketArgs().WithBucket(_options.Bucket),
            ct);
    }

    private static string SanitizeFileName(string fileName)
        => fileName.Replace(" ", "_");
}