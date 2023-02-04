using System.Linq;
using ModelzAndUtils.Enums;
using Telegram.Bot.Types.Enums;

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
    public string Text { get; } = string.Empty;
    public SourceProcessors SourceProcessor { get; }
    public int ReplyMessageId { get; }
    public InputMediaType AttachmentMediaMediaType { get; }

    public UsersPermissions AllowedReceivers { get; } =
        UsersPermissions.None; // TODO: Do we need separate permission to direct messages even if we send with ChatID?

    public IReadOnlyList<TelegramFile> Attachment { get; set; }

    public IReadOnlyList<TelegramButton> Keyboard { get; set; }

    public bool IsKeyboardInline { get; set; }

    public string InlineKeyboardPrefix { get; set; } = string.Empty;

    public OutgoingMessage(
        OutgoingMessageType type,
        UsersPermissions allowedReceivers,
        SourceProcessors sourceProcessor,
        int replyMessageId = 0)
    {
        Type = type;
        AllowedReceivers = allowedReceivers;
        SourceProcessor = sourceProcessor;
        ReplyMessageId = replyMessageId;
    }

    public OutgoingMessage(
        OutgoingMessageType type,
        string message,
        SourceProcessors sourceProcessor,
        int replyMessageId = 0)
    {
        Type = type;
        Text = message;
        SourceProcessor = sourceProcessor;
        ReplyMessageId = replyMessageId;
    }

    public OutgoingMessage(
        long chatId,
        OutgoingMessageType type,
        string message,
        SourceProcessors sourceProcessor,
        int replyMessageId = 0)
    {
        ChatId = chatId;
        Text = message;
        Type = type;
        SourceProcessor = sourceProcessor;
        ReplyMessageId = replyMessageId;
    }

    public OutgoingMessage(long chatId, string message, SourceProcessors sourceProcessor,
        int replyMessageId = 0)
    {
        ChatId = chatId;
        Text = message;
        Type = OutgoingMessageType.Text;
        SourceProcessor = sourceProcessor;
        ReplyMessageId = replyMessageId;
    }

    public OutgoingMessage(long chatId, OutgoingMessageType type, SourceProcessors sourceProcessor,
        int replyMessageId = 0)
    {
        ChatId = chatId;
        Text = string.Empty;
        Type = type;
        SourceProcessor = sourceProcessor;
        ReplyMessageId = replyMessageId;
    }

    public OutgoingMessage(
        UsersPermissions allowedReceivers,
        IReadOnlyList<TelegramFile> attachment,
        InputMediaType mediaType,
        SourceProcessors sourceProcessors,
        int replyMessageId = 0)
    {
        Type = OutgoingMessageType.Attachment;
        AttachmentMediaMediaType = mediaType;
        SourceProcessor = sourceProcessors;
        ReplyMessageId = replyMessageId;
        Attachment = attachment;
        AllowedReceivers = allowedReceivers;
    }

    public OutgoingMessage(
        long chatId,
        IReadOnlyList<TelegramFile> attachment,
        InputMediaType mediaType,
        SourceProcessors sourceProcessors,
        int replyMessageId = 0)
    {
        ChatId = chatId;
        Type = OutgoingMessageType.Attachment;
        AttachmentMediaMediaType = mediaType;
        SourceProcessor = sourceProcessors;
        ReplyMessageId = replyMessageId;
        Attachment = attachment;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var receiver = DirectMessage ? ChatId.ToString() : Constants.ALL;
        var permissions = AllowedReceivers == UsersPermissions.System ? string.Empty : $", Permissions: {AllowedReceivers.ToString()}";
        var attachments = Attachment.Count > 0 ? string.Empty : $", Permissions: {Attachment.Count}";
        return
            $"MessageType: {Type.AsString()} ({SourceProcessor.AsString()}), Receiver: {receiver}, Message: {Text}{permissions}{attachments}";
    }
}