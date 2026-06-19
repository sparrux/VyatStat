using Microsoft.Extensions.Caching.Memory;

namespace Identity.WebAPI.Authentication;

static class SecurityStampCache
{
    public static string Key(Guid userId) => $"security_stamp:{userId}";

    public static string Key(string userId) => $"security_stamp:{userId}";

    public static void Invalidate(IMemoryCache cache, Guid userId) =>
        cache.Remove(Key(userId));
}
