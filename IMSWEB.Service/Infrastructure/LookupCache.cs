using System;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Web;

namespace IMSWEB.Service.Infrastructure
{
    public static class LookupCache
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public static T GetOrCreate<T>(string key, Func<T> factory, TimeSpan ttl)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return factory();
            }

            if (Cache.Get(key) is T cached)
            {
                return cached;
            }

            var value = factory();
            Cache.Set(key, value, DateTimeOffset.UtcNow.Add(ttl));
            return value;
        }

        public static void RemoveByPrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return;
            }

            foreach (var item in Cache)
            {
                if (item.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    Cache.Remove(item.Key);
                }
            }
        }

        public static int? GetConcernId()
        {
            var identity = HttpContext.Current?.User?.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst("ConcernID");
            if (claim == null)
            {
                return null;
            }

            if (int.TryParse(claim.Value, out var concernId))
            {
                return concernId;
            }

            return null;
        }

        public static string BuildTenantKey(string baseKey, int? concernId)
        {
            return concernId.HasValue && concernId.Value > 0
                ? $"{baseKey}:{concernId.Value}"
                : baseKey;
        }
    }
}
