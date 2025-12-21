namespace PositiveNewsPlatform.Domain.Articles;

public class Article
{
    public ArticleId Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public ArticleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Article() { }

    private Article(
        ArticleId id,
        string title,
        string content,
        ArticleStatus status)
    {
        Id = id;
        Title = title;
        Content = content;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    public static Article Create(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty");

        return new Article(
            ArticleId.New(),
            title,
            content,
            ArticleStatus.Draft
        );
    }
    
    public void Update(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty");

        Title = title;
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }


    public void Publish()
    {
        Status = ArticleStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }
}