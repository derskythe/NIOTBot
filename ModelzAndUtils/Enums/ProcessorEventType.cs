using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ModelzAndUtils.Enums;

[Serializable]
public enum ProcessorEventType
{
    [EnumMember] Unknown,
    [EnumMember] Message,
    [EnumMember] BotStoped,
    [EnumMember] BotStarted,
    [EnumMember] Menu,
    [EnumMember] AddUser,
    [EnumMember] RemoveUser,
    [EnumMember] RuntimeError,
    [EnumMember] Tick,
    [EnumMember] OrhainedMessage,
    [EnumMember] NeverBelivied
}