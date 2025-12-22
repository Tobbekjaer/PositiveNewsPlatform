namespace PositiveNewsPlatform.Api.Contracts.Articles;

public sealed record CreateArticleRequest(
    string Title,
    string Content
);