namespace PositiveNewsPlatform.Infrastructure.Persistence.ReadSide.Redis;

public static class RedisKeys
{
    public static string Article(Guid id) => $"articles:{id}";
    public static string Latest(int take) => $"articles:latest:{take}";
}