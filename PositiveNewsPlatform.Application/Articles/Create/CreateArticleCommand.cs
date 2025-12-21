namespace PositiveNewsPlatform.Application.Articles.Create;

public sealed record CreateArticleCommand(
    string Title,
    string Content,
    UploadImageRequest? Image
);

public sealed record UploadImageRequest(
    Stream Content,
    string FileName,
    string ContentType
);
