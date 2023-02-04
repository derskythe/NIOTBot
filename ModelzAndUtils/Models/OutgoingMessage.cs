using ModelzAndUtils.Enums;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace ModelzAndUtils.Models;

public class OutgoingMessage
{
    public bool DirectMessage => ChatId > 0;
    public long ChatId { get; set; }
    public OutgoingMessageType Type { get; set; }
    public string Message { get; } = string.Empty;
    public SourceProcessors SourceProcessor { get; }
    public UsersPermissions AllowedReceivers { get; } = UsersPermissions.None; // TODO: Do we need separate permission to direct messages even if we send with ChatID?
    public IReadOnlyList<byte[]> Attachment { get; set; }    
    public IReadOnlyList<TelegramButton> Keyboard { get; set; }
    
    public bool IsKeyboardInline { get; set; }
    
    public string InlineKeyboardPrefix { get; set; } = string.Empty;

    public OutgoingMessage(OutgoingMessageType type, UsersPermissions allowedReceivers, SourceProcessors sourceProcessor)
    {
        Type = type;
        AllowedReceivers = allowedReceivers;
        SourceProcessor = sourceProcessor;
    }

    public OutgoingMessage(OutgoingMessageType type, string message, SourceProcessors sourceProcessor)
    {
        Type = type;
        Message = message;
        SourceProcessor = sourceProcessor;
    }

    public OutgoingMessage(long chatId, OutgoingMessageType type, string message, SourceProcessors sourceProcessor)
    {
        ChatId = chatId;
        Message = message;
        Type = type;
        SourceProcessor = sourceProcessor;
    }
    
    public OutgoingMessage(long chatId, string message, SourceProcessors sourceProcessor)
    {
        ChatId = chatId;
        Message = message;
        Type = OutgoingMessageType.Text;
        SourceProcessor = sourceProcessor;
    }

    public OutgoingMessage(long chatId, OutgoingMessageType type, SourceProcessors sourceProcessor)
    {
        ChatId = chatId;
        Message = string.Empty;
        Type = type;
        SourceProcessor = sourceProcessor;
    }

    public OutgoingMessage(UsersPermissions allowedReceivers, IReadOnlyList<byte[]> attachment, SourceProcessors sourceProcessors)
    {
        Type = OutgoingMessageType.Attachment;
        SourceProcessor = sourceProcessors;
        Attachment = attachment;
        AllowedReceivers = allowedReceivers;
    }
    
    public OutgoingMessage(long chatId, IReadOnlyList<byte[]> attachment, SourceProcessors sourceProcessors)
    {
        ChatId = chatId;
        Type = OutgoingMessageType.Attachment;
        SourceProcessor = sourceProcessors;
        Attachment = attachment;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var receiver = DirectMessage ? ChatId.ToString() : Constants.ALL;
        var permissions = AllowedReceivers == UsersPermissions.System ? string.Empty : $", Permissions: {AllowedReceivers.ToString()}";
        var attachments = Attachment.Count > 0 ? string.Empty : $", Permissions: {Attachment.Count}";
        return $"MessageType: {Type.AsString()} ({SourceProcessor.AsString()}), Receiver: {receiver}, Message: {Message}{permissions}{attachments}";
    }
}