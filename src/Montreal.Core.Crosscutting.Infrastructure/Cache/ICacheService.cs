using System;

namespace Montreal.Core.Crosscutting.Infrastructure.Cache
{
    public interface ICacheService
    {
        T Get<T>(string key);
        T Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, TimeSpan? slidingExpiration);
    }
}
