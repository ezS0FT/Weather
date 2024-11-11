using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace Weather
{
    public static class SimpleMemoryCache
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        public static void AddToCache(string key, string value, int expirationMinutes = 1)
        {
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(expirationMinutes) };
            Cache.Set(key, value, policy);
        }
        public static string GetFromCache(string key)
        {
            return Cache.Contains(key) ? Cache[key] as string : null;
        }
    }
}
