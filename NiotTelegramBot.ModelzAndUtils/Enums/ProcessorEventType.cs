using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace NiotTelegramBot.ModelzAndUtils.Enums;

[Serializable]
public enum ProcessorEventType
{
    [EnumMember(Value = "Unknown")] Unknown,
    [EnumMember(Value = "Message")] Message,
    [EnumMember(Value = "Bot Stoped")] BotStoped,
    [EnumMember(Value = "Bot started")] BotStarted,
    [EnumMember(Value = "Menu")] Menu,
    [EnumMember(Value = "Add user")] AddUser,
    [EnumMember(Value = "Remove user")] RemoveUser,
    [EnumMember(Value = "Runtime error")] RuntimeError,
    [EnumMember(Value = "Tick")] Tick,
    [EnumMember(Value = "Orhained message")] OrhainedMessage,
    [EnumMember(Value = "Never belivied")] NeverBelivied
}