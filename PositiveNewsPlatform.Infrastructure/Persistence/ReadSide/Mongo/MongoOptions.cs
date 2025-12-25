namespace PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Mongo;

public sealed class MongoOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = "PositiveNewsReadDb";
    
    // Toggles
    public string ReadPreference { get; set; } = "Primary";        // Primary | SecondaryPreferred | Nearest
    public string WriteConcern { get; set; } = "Majority";         // Majority | W1
}