namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql.Models;

public sealed class MediaRegistryWriteModel
{
    public Guid MediaId { get; set; }
    public Guid ArticleId { get; set; }

    public string ObjectKey { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime UploadedAtUtc { get; set; }
}