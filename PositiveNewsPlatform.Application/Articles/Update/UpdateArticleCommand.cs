namespace PositiveNewsPlatform.Application.Articles.Update;

public sealed record UpdateArticleCommand(
    Guid ArticleId,
    string Title,
    string Content
);
