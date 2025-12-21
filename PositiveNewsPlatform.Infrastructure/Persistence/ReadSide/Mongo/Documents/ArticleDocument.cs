using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Mongo.Documents;

public sealed class ArticleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid ArticleId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "Draft";

    [BsonElement("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; }

    [BsonElement("updatedAtUtc")]
    public DateTime? UpdatedAtUtc { get; set; }

    // Media kommer senere - vi starter med en tom liste eller ingen felt.
}