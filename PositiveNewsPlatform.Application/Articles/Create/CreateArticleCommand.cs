namespace PositiveNewsPlatform.Application.Articles.Create;

public sealed record CreateArticleCommand(
    string Title,
    string Content,
    UploadImageRequest? Image
);

public sealed record UploadImageRequest(
    Stream Content,
    long SizeBytes,
    string FileName,
    string ContentType
);
