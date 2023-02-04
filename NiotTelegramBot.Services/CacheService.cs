using System.Runtime.CompilerServices;
using EnumsNET;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ModelzAndUtils.Enums;
using ModelzAndUtils.Interfaces;
using ModelzAndUtils.Models;

// ReSharper disable MemberCanBePrivate.Global

namespace NiotTelegramBot.Services;

/// <inheritdoc />
public class CacheService : ICacheService
{
    // ReSharper disable once InconsistentNaming
    private readonly ILogger<CacheService> Log;
    private readonly IMessageQueueService _MessageQueueService;
    private readonly IMemoryCache _MemoryCache = new MemoryCache(new MemoryCacheOptions());

    public CacheService(ILogger<CacheService> log, IMessageQueueService messageQueueService)
    {
        Log = log;
        _MessageQueueService = messageQueueService;
    }

    /// <inheritdoc />
    public void RemoveFromCacheIfExists(CacheKeys key, string username)
    {
        var cacheKey = FormatCacheKey(key, username);
        RemoveFromCacheIfExistByCacheValue(cacheKey);
    }

    /// <inheritdoc />
    public void RemoveFromCacheIfExists(CacheKeys key)
    {
        RemoveFromCacheIfExists(key, string.Empty);
    }

    private void RemoveFromCacheIfExistByCacheValue(string cacheKey)
    {
        if (_MemoryCache.TryGetValue(cacheKey, out string _))
        {
            _MemoryCache.Remove(cacheKey);
        }
    }
    
    /// <inheritdoc />
    public void SetStringCache(string value, TimeSpan expiration, CacheKeys key, string username)
    {
        var cacheKey = FormatCacheKey(key, username);
        RemoveFromCacheIfExistByCacheValue(cacheKey);
        _MemoryCache.Set(cacheKey, value, expiration);
    }
    
    /// <inheritdoc />
    public void SetStringCache(string value, CacheKeys key, string username)
    {
        SetStringCache(value, TimeSpan.FromHours(24), key, username);
    }
    
    /// <inheritdoc />
    public void SetCache<T>(T value, TimeSpan expiration, CacheKeys key, string username)
        where T : class
    {
        var cacheKey = FormatCacheKey(key, username);
        RemoveFromCacheIfExistByCacheValue(cacheKey);
        _MemoryCache.Set(cacheKey, value, expiration);
    }
    
    /// <inheritdoc />
    public void SetCache<T>(T value, TimeSpan expiration, CacheKeys key)
        where T : class
    {
        SetCache(value, expiration, key, string.Empty);
    }

    /// <inheritdoc />
    public void SetCache<T>(T value, CacheKeys key)
        where T : class
    {
        SetCache(value, TimeSpan.FromHours(3), key, string.Empty);
    }

    /// <inheritdoc />
    public void SetCache<T>(T value, CacheKeys key, string username)
        where T : class
    {
        SetCache(value, TimeSpan.FromHours(3), key, username);
    }

    /// <inheritdoc />
    public (bool ExistsInCache, T Value) GetCache<T>(CacheKeys key, string username)
        where T : class, new()
    {
        var cacheKey = FormatCacheKey(key, username);
        try
        {
            if (_MemoryCache.TryGetValue(cacheKey, out T? value))
            {
                return value == null ? (true, new T()) : (true, value);
            }
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
            _MessageQueueService.ProcessEnqueue(new MessageProcess(ProcessorEventType.RuntimeError, exp.Message));
        }

        return (false, new T());
    }

    /// <inheritdoc />
    public (bool ExistsInCache, T Value) GetCache<T>(CacheKeys key)
        where T : class, new()
    {
        var result = GetCache<T>(key, string.Empty);
        return (result.ExistsInCache, result.Value);
    }

    /// <inheritdoc />
    public (bool ExistsInCache, IEnumerable<T> Value) GetCacheList<T>(CacheKeys key, string username)
        where T : class
    {
        var cacheKey = FormatCacheKey(key, username);
        try
        {
            if (_MemoryCache.TryGetValue(cacheKey, out List<T>? value))
            {
                return value == null ? (true, new List<T>()) : (true, value);
            }
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
            _MessageQueueService.ProcessEnqueue(new MessageProcess(ProcessorEventType.RuntimeError, exp.Message));
        }

        return (false, new List<T>());
    }

    /// <inheritdoc />
    public (bool ExistsInCache, IEnumerable<T> Value) GetCacheList<T>(CacheKeys key)
        where T : class
    {
        return GetCacheList<T>(key, string.Empty);
    }
    
    /// <inheritdoc />
    public (bool ExistsInCache, string Value) GetStringCache(CacheKeys key)
    {
        var result = GetStringCache(key, string.Empty);
        return (result.ExistsInCache, result.Value);
    }
    
    /// <inheritdoc />
    public (bool ExistsInCache, string Value) GetStringCache(CacheKeys key, string username)
    {
        var cacheKey = FormatCacheKey(key, username);
        try
        {
            if (_MemoryCache.TryGetValue(cacheKey, out string? value))
            {
                return string.IsNullOrEmpty(value) ? (true, string.Empty) : (true, value);
            }
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
            _MessageQueueService.ProcessEnqueue(new MessageProcess(ProcessorEventType.RuntimeError, exp.Message));
        }

        return (false, string.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatCacheKey(CacheKeys key, string username)
    {
        return string.IsNullOrEmpty(username) ? key.AsString() : $"{key.AsString()}_{username}";
    }
}