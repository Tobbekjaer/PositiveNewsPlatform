namespace PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Redis;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; set; } = string.Empty;
}