using Microsoft.AspNetCore.Mvc;
using PositiveNewsPlatform.Api.Contracts.Articles;
using PositiveNewsPlatform.Application.Articles.Create;
using PositiveNewsPlatform.Application.Articles.GetById;
using PositiveNewsPlatform.Application.Articles.GetLatest;
using PositiveNewsPlatform.Application.Articles.Update;

namespace PositiveNewsPlatform.Api.Controllers;

[ApiController]
[Route("api/articles")]
public sealed class ArticlesController : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(CreateArticleResult), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromForm] CreateArticleForm request,
        [FromServices] CreateArticleHandler handler,
        CancellationToken ct)
    {
        UploadImageRequest? image = null;

        if (request.Image is not null)
        {
            image = new UploadImageRequest(
                Content: request.Image.OpenReadStream(),
                SizeBytes: request.Image.Length,
                FileName: request.Image.FileName,
                ContentType: request.Image.ContentType
            );
        }

        var result = await handler.HandleAsync(
            new CreateArticleCommand(
                Title: request.Title,
                Content: request.Content,
                Image: image
            ),
            ct);

        return CreatedAtAction(nameof(GetById), new { id = result.ArticleId }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateArticleResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateArticleRequest request,
        [FromServices] UpdateArticleHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(
            new UpdateArticleCommand(
                ArticleId: id,
                Title: request.Title,
                Content: request.Content
            ),
            ct);

        return result.Updated ? Ok(result) : NotFound();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] GetArticleByIdHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetArticleByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("latest")]
    [ProducesResponseType(typeof(IReadOnlyList<ArticleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLatest(
        [FromQuery] int take,
        [FromServices] GetLatestArticlesHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetLatestArticlesQuery(take <= 0 ? 10 : take), ct);
        return Ok(result);
    }
}
