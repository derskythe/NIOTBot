using EnumsNET;
using Microsoft.Extensions.Logging;
using ModelzAndUtils;
using ModelzAndUtils.Enums;
using ModelzAndUtils.Interfaces;
using ModelzAndUtils.Models;

// ReSharper disable UnusedMember.Local

namespace NiotTelegramBot.Services;

/// <inheritdoc />
public class ChatUserService : IChatUsers
{
    // ReSharper disable once InconsistentNaming
    private readonly ILogger<ChatUserService> Log;
    private readonly IMessageQueueService _MessageQueueService;
    private readonly ICacheService _CacheService;
    private readonly string _FilePath;
    private readonly Lazy<Dictionary<string, TelegramUser>> _Users;
    private readonly SemaphoreSlim _Semaphore = new(1);

    public ChatUserService(ILogger<ChatUserService> log, IMessageQueueService messageQueueService, ICacheService cacheService)
    {
        Log = log;
        _MessageQueueService = messageQueueService;
        _CacheService = cacheService;

        _FilePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "config", "users.json");
        if (!File.Exists(_FilePath))
        {
            Log.LogWarning("Can't find users file, creating empty one");
            File.WriteAllText(_FilePath, new List<TelegramUser>(0).ToJsonIntended());
        }

        Log.LogInformation("Path for users file: {Path}", _FilePath);
        _Users = new Lazy<Dictionary<string, TelegramUser>>(() =>
        {
            Dictionary<string, TelegramUser> result;
            try
            {
                result = File.ReadAllText(_FilePath)
                             .FromJson<List<TelegramUser>>()
                             .ToDictionary(s => s.Username);
            }
            catch (Exception exp)
            {
                Log.LogError(exp, "Load failed! Message: {Message}", exp.Message);
                result = new Dictionary<string, TelegramUser>();
            }

            if (result.Count == 0)
            {
                Log.LogWarning("Auth user list is empty!");
            }
            else
            {
                Log.LogInformation("Loaded {Count} users", result.Count);
            }

            return result;
        });
    }

    private T GetItemByWithoutId<T>(Func<T> callMethod, CacheKeys key, TimeSpan timeSpan)
        where T : class, new()
    {
        try
        {
            var cacheEntry = _CacheService.GetCache<T>(key);

            if (cacheEntry.ExistsInCache)
            {
                return cacheEntry.Value;
            }

            cacheEntry.Value = callMethod();
            _CacheService.SetCache(cacheEntry.Value, timeSpan, key);

            return cacheEntry.Value;
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
        }

        return new T();
    }

    private IEnumerable<T> GetItemById<T>(
        Func<long, IEnumerable<T>> callMethod,
        long id,
        CacheKeys key,
        TimeSpan timeSpan)
        where T : class
    {
        try
        {
            var username = id.ToString();
            var cacheEntry = _CacheService.GetCacheList<T>(key, username);

            if (cacheEntry.ExistsInCache)
            {
                return cacheEntry.Value;
            }

            cacheEntry.Value = callMethod(id);
            var cacheEntryValue = cacheEntry.Value.ToList();
            _CacheService.SetCache(cacheEntryValue,
                                   timeSpan,
                                   key,
                                   username);

            return cacheEntryValue;
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
        }

        return new List<T>();
    }

    private IEnumerable<TelegramUser> ListUsers(long id)
    {
        var permissions = (UsersPermissions)id;
        return _Users.Value.Select(i => i.Value)
                     .Where(i =>
                                i.Permission == permissions && i.ChatId > 0);
    }

    /// <inheritdoc />
    public List<TelegramUser> ListUsersByPermission(UsersPermissions permission)
    {
        return GetItemById(ListUsers,
                           (long)permission,
                           CacheKeys.UsersPermissions,
                           TimeSpan.FromHours(24))
            .ToList();
    }

    /// <inheritdoc />
    public bool IsAuthorized(string username)
    {
        return _Users.Value.ContainsKey(username);
    }

    /// <inheritdoc />
    public async Task AddUser(string username, long chatId)
    {
        Log.LogInformation("Add new username: {Username}", username);
        _Users.Value.Add(username,
                         new TelegramUser()
                         {
                             Username = username,
                             ChatId = chatId
                         });
        _MessageQueueService.ProcessEnqueue(new MessageProcess(
                                                               ProcessorEventType.AddUser,
                                                               username));
        await SaveToFile();
    }

    /// <inheritdoc />
    public async Task RemoveUser(string username)
    {
        Log.LogInformation("Delete username: {Username}", username);
        _Users.Value.Remove(username);
        _MessageQueueService.ProcessEnqueue(new MessageProcess(
                                                               ProcessorEventType.RemoveUser,
                                                               username));
        await SaveToFile();
    }

    /// <inheritdoc />
    public TelegramUser GetByUsername(string username)
    {
        return _Users.Value[username];
    }

    /// <inheritdoc />
    public TelegramUser GetByChatId(long chatId)
    {
        IReadOnlyList<TelegramUser> TelegramUser(long localChatId)
        {
            var telegramUser = _Users.Value.FirstOrDefault(
                                                           u => u.Value.ChatId == localChatId)
                                     .Value;
            return new []{telegramUser};
        }

        return GetItemById(TelegramUser,
                           chatId,
                           CacheKeys.UsersByChatId,
                           TimeSpan.FromHours(24)).First();
    }

    /// <inheritdoc />
    public bool HasPermission(string username, UsersPermissions permissions)
    {
        return _Users.Value[username].Permission == permissions;
    }

    private async Task SaveToFile()
    {
        var obtainResult = await _Semaphore.WaitAsync(TimeSpan.FromMinutes(1));
        var json = _Users.Value.Select(s => s.Value).ToList().ToJsonIntended();
        if (!obtainResult)
        {
            Log.LogCritical("Can't obtain Semaphore after 1 min of waiting! User list not saved! JSON: \n{Json}", json);
        }

        try
        {
            await File.WriteAllTextAsync(_FilePath, json);
            Log.LogInformation("Users saved");
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "Save failed! Message: {Message}, JSON: \n{Json}", exp.Message, json);
        }
        finally
        {
            _Semaphore.Release();
        }
    }
}