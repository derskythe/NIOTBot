#nullable enable
using ModelzAndUtils.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace ModelzAndUtils.Models;

public class MessageProcess
{
    public ProcessorEventType Type { get; set; }
    public long IncomingMessageChatId { get; set; }
    public bool IsOrphained { get; set; }
    public int TryCount { get; set; }
    public Update? Update { get; set; }
    public string AdditionalInfo { get; set; }
    public string StickMessage { get; set; } = string.Empty;

    public MessageProcess()
    {
        AdditionalInfo = string.Empty;
        Update = null;
        Type = ProcessorEventType.Unknown;
    }

    public MessageProcess(long incomingMessageChatId, Update update)
    {
        AdditionalInfo = string.Empty;
        Type = ProcessorEventType.Message;
        Update = update;
        
        IncomingMessageChatId = incomingMessageChatId;        
    }

    public MessageProcess(ProcessorEventType type, string additionalInfo)
    {
        Update = null;
        Type = type;
        AdditionalInfo = additionalInfo;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var update = Update == null ? UpdateType.Unknown.AsString() : Update?.Type.AsString();
        var orphained = IsOrphained ? $" ({ProcessorEventType.OrhainedMessage.AsString()}!)" : string.Empty;
        var stickMessage = !string.IsNullOrEmpty(StickMessage) ? $", StickMessage: {StickMessage}" : string.Empty;
        return $"Type: {Type.AsString()}{orphained}, UpdateType: {update}, AdditionalInfo: {AdditionalInfo}{stickMessage}";
    }
}