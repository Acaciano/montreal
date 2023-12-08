using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

namespace Montreal.Core.Crosscutting.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            var value = _cache.GetString(key);

            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public T Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, TimeSpan? slidingExpiration)
        {
            if (absoluteExpirationRelativeToNow == null) absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
            if (slidingExpiration == null) slidingExpiration = TimeSpan.FromMinutes(30);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
                SlidingExpiration = slidingExpiration
            };

            _cache.SetString(key, JsonConvert.SerializeObject(value), options);

            return value;
        }

        public void Remove<T>(string key)
        {
            _cache.Remove(key);
        }
    }
}
