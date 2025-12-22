namespace PositiveNewsPlatform.Api.Contracts.Articles;

public sealed record UpdateArticleRequest(
    string Title,
    string Content
);