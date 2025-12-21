namespace PositiveNewsPlatform.Application.Articles.GetById;

public sealed record ArticleDto(
    Guid ArticleId,
    string Title,
    string Content,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyList<MediaDto> Media
);

public sealed record MediaDto(
    string ObjectKey,
    string MimeType,
    long SizeBytes
);