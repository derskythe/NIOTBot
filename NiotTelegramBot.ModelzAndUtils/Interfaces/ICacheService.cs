using NiotTelegramBot.ModelzAndUtils.Enums;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace NiotTelegramBot.ModelzAndUtils.Interfaces;

public interface ICacheService
{
    void RemoveFromCacheIfExists(CacheKeys key, string username);

    void RemoveFromCacheIfExists(CacheKeys key);

    public void SetStringCache(string value, TimeSpan expiration, CacheKeys key, string username);

    public void SetStringCache(string value, CacheKeys key, string username);

    void SetCache<T>(T value, TimeSpan expiration, CacheKeys key, string username)
        where T : class;

    void SetCache<T>(T value, TimeSpan expiration, CacheKeys key)
        where T : class;

    void SetCache<T>(T value, CacheKeys key)
        where T : class;

    void SetCache<T>(T value, CacheKeys key, string username)
        where T : class;

    (bool ExistsInCache, T Value) GetCache<T>(CacheKeys key, string username)
        where T : class, new();

    (bool ExistsInCache, T Value) GetCache<T>(CacheKeys key)
        where T : class, new();
    
    (bool ExistsInCache, IEnumerable<T> Value) GetCacheList<T>(CacheKeys key, string username)
        where T : class;

    (bool ExistsInCache, IEnumerable<T> Value) GetCacheList<T>(CacheKeys key)
        where T : class;

    public (bool ExistsInCache, string Value) GetStringCache(CacheKeys key);
    
    public (bool ExistsInCache, string Value) GetStringCache(CacheKeys key, string username);
}