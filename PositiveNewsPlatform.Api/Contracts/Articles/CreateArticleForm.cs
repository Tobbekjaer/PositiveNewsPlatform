using Microsoft.AspNetCore.Http;

namespace PositiveNewsPlatform.Api.Contracts.Articles;

public sealed class CreateArticleForm
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}