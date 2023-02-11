using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global

namespace NiotTelegramBot.ModelzAndUtils.Enums;

[Serializable]
public enum CacheKeys
{
    [EnumMember] None,
    [EnumMember] LatestAction,
    [EnumMember] UsersPermissions,
    [EnumMember] UsersByChatId,
    [EnumMember] Processors,
}