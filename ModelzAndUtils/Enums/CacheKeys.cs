using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global

namespace ModelzAndUtils.Enums;

[Serializable]
public enum CacheKeys
{
    [EnumMember] None,
    [EnumMember] LatestAction,
    [EnumMember] UsersPermissions,
}