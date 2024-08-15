using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global

namespace NiotTelegramBot.ModelzAndUtils.Enums;

[Serializable]
public enum OutgoingMessageType
{
    [EnumMember] None,
    [EnumMember] Text,
    [EnumMember] Attachment,
    [EnumMember] Typing,
    [EnumMember] EndTyping,
}