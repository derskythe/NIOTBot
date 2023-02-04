using EnumsNET;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace NiotTelegramBot.Services;

public partial class BotService : BackgroundService
{
    #region ProcessMessage

    private async Task SendAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_Running)
            {
                var message = _MessageQueue.OutgoingDequeue();

                if (message != null)
                {
                    try
                    {
                        switch (message.Type)
                        {
                            case OutgoingMessageType.Text:
                                await SendTextMessage(message, cancellationToken);
                                break;
                            case OutgoingMessageType.Typing:
                                await SendTyping(message, cancellationToken);
                                break;
                            case OutgoingMessageType.Attachment:
                                await SendAttachment(message, cancellationToken);
                                break;
                            case OutgoingMessageType.None:
                            case OutgoingMessageType.EndTyping:
                            default:
                                Log.LogWarning("Invalid message type: {Type}, ignoring",
                                               message.Type.AsString());
                                break;
                        }
                    }
                    catch (TaskCanceledException exp)
                    {
                        Log.LogWarning("{Message}", exp.Message);
                    }
                    catch (Exception exp)
                    {
                        Log.LogError(exp,
                                     "OutgoingMessage: {Outgoing}\nError: {Message}",
                                     message.ToString(),
                                     exp.Message);
                    }
                }
            }

            await Task.Delay(_TimeoutDelaySend, cancellationToken);
        }
    }

    #endregion

    private async Task SendTyping(OutgoingMessage message, CancellationToken cancellationToken)
    {
        IReadOnlyList<TelegramUser> userList;
        if (message.DirectMessage)
        {
            var user = _ChatUsers.GetByChatId(message.ChatId);

            if (user == null || user.ChatId == 0)
            {
                Log.LogWarning("Can't send obtain user with: {ChatId}", message.ChatId);
                return;
            }

            userList = new[]
            {
                user
            };
        }
        else
        {
            userList = _ChatUsers.ListUsersByPermission(message.AllowedReceivers);
        }

        foreach (var user in userList)
        {
            if (user.ChatId == 0)
            {
                Log.LogWarning("Can't send status to user: {ChatId}/{Username}",
                               user.ChatId,
                               user.Username);
                continue;
            }

            try
            {
                await _Bot.SendChatActionAsync(user.ChatId,
                                               ChatAction.Typing,
                                               cancellationToken: cancellationToken);
            }
            catch (Exception exp)
            {
                Log.LogError(exp,
                             "Send message failed. ChatAction: {ChatAction}, Username: {Username}({ChatId}), Error: {Message}",
                             ChatAction.Typing.AsString(),
                             user.Username,
                             user.ChatId,
                             exp.Message);
            }
        }
    }

    private async Task SendTextMessage(OutgoingMessage message, CancellationToken cancellationToken)
    {
        var keyboardMarkup = KeyboardMarkup(message);
        var chatId = message.ChatId;
        IReadOnlyList<TelegramUser> userList;
        if (message.DirectMessage)
        {
            userList = new[] { _ChatUsers.GetByChatId(message.ChatId) };
        }
        else
        {
            userList = _ChatUsers.ListUsersByPermission(message.AllowedReceivers);
        }

        foreach (var user in userList)
        {
            try
            {
                await _Bot.SendTextMessageAsync(chatId,
                                                message.Text,
                                                ParseMode.MarkdownV2,
                                                replyToMessageId: message.ReplyMessageId > 0 ? message.ReplyMessageId : null,
                                                replyMarkup: keyboardMarkup,
                                                cancellationToken: cancellationToken);
            }
            catch (Exception exp)
            {
                Log.LogError(exp,
                             "Send message failed. Text: {Text}, Username: {Username}({ChatId}), Error: {Message}",
                             message.Text,
                             user.Username,
                             user.ChatId,
                             exp.Message);
            }
        }
    }

    private async Task SendAttachment(OutgoingMessage message, CancellationToken cancellationToken)
    {
        var keyboardMarkup = KeyboardMarkup(message);
        var chatId = message.ChatId;
        IReadOnlyList<TelegramUser> userList;
        if (message.DirectMessage)
        {
            userList = new[] { _ChatUsers.GetByChatId(message.ChatId) };
        }
        else
        {
            userList = _ChatUsers.ListUsersByPermission(message.AllowedReceivers);
        }

        using var memoryStreamList = new MemoryStreamList();
        memoryStreamList.AddTelegramFile(message.Attachment);

        foreach (var user in userList)
        {
            try
            {
                switch (message.AttachmentMediaMediaType)
                {
                    case InputMediaType.Photo:
                        foreach (var file in memoryStreamList.List)
                        {
                            await _Bot.SendMediaGroupAsync(chatId,
                                                           memoryStreamList.List,
                                                           replyToMessageId: message.ReplyMessageId > 0 ? message.ReplyMessageId : null,
                                                           cancellationToken: cancellationToken);
                        }

                        break;

                    case InputMediaType.Document:
                        foreach (var file in memoryStreamList.List)
                        {
                            await _Bot.SendDocumentAsync(chatId,
                                                         file.Media,
                                                         replyToMessageId: message.ReplyMessageId > 0 ? message.ReplyMessageId : null,
                                                         cancellationToken: cancellationToken);
                            await Task.Delay(100, cancellationToken); // Hold-on
                        }

                        break;
                    default:
                        Log.LogError("Can't send attachment file type: {Type}",
                                     message.AttachmentMediaMediaType.AsString());
                        break;
                }
            }
            catch (Exception exp)
            {
                Log.LogError(exp,
                             "Send message failed. Text: {Text}, Username: {Username}({ChatId}), Error: {Message}",
                             message.Text,
                             user.Username,
                             user.ChatId,
                             exp.Message);
            }
        }
    }
}