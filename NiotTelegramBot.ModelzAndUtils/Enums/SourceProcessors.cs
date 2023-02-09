using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global

namespace NiotTelegramBot.ModelzAndUtils.Enums;

[Serializable]
public enum SourceProcessors
{
    [EnumMember] None,
    [EnumMember] DummyProcessor,
    [EnumMember] EchoProcessor,
    [EnumMember] FileProcessor,
    [EnumMember] RuntimeErrorProcessor,
    [EnumMember] StartStopInfoProcessor,
    [EnumMember] UserAuditProcessor,
    [EnumMember] DefaultMessagesProcessor,
    [EnumMember] DockerProcessor,
    [EnumMember] InvalidProcessor,
}