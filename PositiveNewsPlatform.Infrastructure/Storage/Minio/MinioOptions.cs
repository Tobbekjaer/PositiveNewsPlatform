namespace PositiveNewsPlatform.Infrastructure.Storage.Minio;

public sealed class MinioOptions
{
    public const string SectionName = "Minio";

    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Bucket { get; set; } = "positive-news-media";
    public bool UseSsl { get; set; } = false;
}