using EnumsNET;
using Microsoft.Extensions.Logging;
using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;
using NiotTelegramBot.Plugins;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Emoji = NiotTelegramBot.ModelzAndUtils.Enums.Emoji;

namespace NiotTelegramBot.Services;

public partial class BotService
{
// ReSharper disable once InconsistentNaming
    private readonly ILogger<BotService> Log;

    private bool _IsDisposed;
    private bool _Running;

    private readonly IMessageQueueService _MessageQueue;
    private readonly IChatUsers _ChatUsers;
    private readonly ITelegramBotClient _Bot;
    private readonly ICacheService _Cache;

    private Task _SendTask = Task.CompletedTask;

    private readonly TimeSpan _TimeoutDelaySend = TimeSpan.FromSeconds(1);

    public BotService(
        ILogger<BotService> log,
        ICacheService cache,
        IMessageQueueService messageQueue,
        IChatUsers chatUsers,
        ITelegramBotClient bot)
    {
        Log = log;
        _MessageQueue = messageQueue;
        _ChatUsers = chatUsers;
        _Bot = bot;
        _Cache = cache;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // StartReceiving does not block the caller thread
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _Bot.StartReceiving(updateHandler: HandleUpdateAsync,
                                pollingErrorHandler: HandlePollingErrorAsync,
                                receiverOptions: receiverOptions,
                                cancellationToken: stoppingToken);

            var me = await _Bot.GetMeAsync(cancellationToken: stoppingToken);
            Log.LogInformation("Init bot: {Name}", me.FirstName);
            _Running = true;

            _SendTask = SendAsync(stoppingToken);
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
            {
                return;
            }

            var chatId = message.Chat.Id;
            var username = message.Chat.Username ?? string.Empty;

            if (string.IsNullOrEmpty(username) || !_ChatUsers.IsAuthorized(username))
            {
                Log.LogWarning("User not authorized. ChatID: {ChatId}, Username: {Username}",
                               chatId,
                               username);
                var text = Emoji.Robot.MessageCombine(" Bot\n" +
                                                      $"{Emoji.NoEntry.GetEmoji()} {i18n.ErrorUserNotAuthorized}");
                await botClient.SendTextMessageAsync(
                                                     chatId: chatId,
                                                     text: text,
                                                     cancellationToken: cancellationToken);
                return;
            }

            await _ChatUsers.UpdateChatId(username, chatId);

            Log.LogDebug("Received '{MessageText}' message in chat {ChatId}",
                         MessageType.Text.MessageShort(message.Text),
                         chatId);
            var process = new MessageProcess(chatId, update);
            await _ChatUsers.UpdateChatId(username, chatId);
            _MessageQueue.ProcessEnqueue(process);
            //
            // // Echo received message text
            // Message sentMessage = await botClient.SendTextMessageAsync(
            //                                                            chatId: chatId,
            //                                                            text: "You said:\n" + messageText,
            //                                                            cancellationToken: cancellationToken);
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ApiRequestException apiRequestException)
        {
            Log.LogError("Telegram API Error: [{ErrorCode}], {Message}",
                         apiRequestException.ErrorCode,
                         apiRequestException.Message);
        }
        else
        {
            Log.LogError(exception, "{Message}", exception.Message);
        }

        return Task.CompletedTask;
    }

    #region Keyboard

    private IReplyMarkup? KeyboardMarkup(OutgoingMessage message)
    {
        IReplyMarkup? keyboardMarkup;
        switch (message.Keyboard.Count)
        {
            case > 0 when !message.IsKeyboardInline:
                keyboardMarkup = FormatKeyboard(message.Keyboard);
                break;
            case > 0 when message.IsKeyboardInline:
                if (message.ChatId == 0)
                {
                    // TODO: add ability to send inline keyboard to many users
                    throw new ArgumentException("ChatId cannot be 0 when inline keyboard added");
                }

                keyboardMarkup = FormatInlineSingleLine(message.ChatId,
                                                        message.SourceProcessor.AsString(),
                                                        message.InlineKeyboardPrefix,
                                                        message.Keyboard);
                break;
            default:
                keyboardMarkup = default;
                break;
        }

        return keyboardMarkup;
    }

    private ReplyKeyboardMarkup FormatKeyboard(IReadOnlyList<TelegramButton> buttons, int perRow = 2)
    {
        var rowNum = Convert.ToInt32(Math.Ceiling(buttons.Count / (decimal)perRow));
        var keyboardInline = new KeyboardButton[rowNum][];

        var i = 0;
        for (var row = 0; row < keyboardInline.Length; row++)
        {
            var button = buttons[i];
            if (button.InlineKeyboard)
            {
                Log.LogWarning("Found inline button for text: {Text}",
                               button.Text);
            }

            if (i + 1 < buttons.Count)
            {
                var keyboardButtons = new KeyboardButton[2];
                keyboardButtons[0] = new KeyboardButton(button.Text)
                {
                    Text = button.Text,
                    RequestContact = button.IsPhoneNumberRequest,
                    RequestLocation = button.IsLocationRequest
                };
                i++;
                keyboardButtons[1] = new KeyboardButton(button.Text)
                {
                    Text = button.Text,
                    RequestContact = button.IsPhoneNumberRequest,
                    RequestLocation = button.IsLocationRequest
                };
                keyboardInline[row] = keyboardButtons;
                i++;
            }
            else if (i + 1 == buttons.Count)
            {
                var keyboardButtons = new KeyboardButton[1];
                keyboardButtons[0] = new KeyboardButton(button.Text)
                {
                    Text = button.Text,
                    RequestContact = button.IsPhoneNumberRequest,
                    RequestLocation = button.IsLocationRequest
                };
                i++;
                keyboardInline[row] = keyboardButtons;
            }
            else
            {
                break;
            }
        }

        return new ReplyKeyboardMarkup(keyboardInline)
        {
            ResizeKeyboard = true
        };
    }

    private InlineKeyboardMarkup FormatInlineSingleLine(long chatId, string type, string subType, IReadOnlyList<TelegramButton> buttons)
    {
        var prefix = $"{type};{chatId};{subType};";

        var i = 0;
        var inlineButton = new InlineKeyboardButton[buttons.Count];
        foreach (var row in buttons)
        {
            if (!row.InlineKeyboard)
            {
                Log.LogWarning("Found NOT inline button for text: {Text}, ChatID: {ChatID}, Type: {Type}, Sub: {Sub}",
                               row.Text,
                               chatId,
                               type,
                               subType);
            }

            var keyboardButtons = new InlineKeyboardButton(row.Text)
            {
                Text = row.Text,
                CallbackData = prefix + row.CallbackData
            };
            inlineButton[i] = keyboardButtons;
            i++;
        }

        var keyboardInline = new InlineKeyboardButton[1][];
        keyboardInline[0] = inlineButton;

        return new InlineKeyboardMarkup(keyboardInline);
    }

    #endregion

    /// <inheritdoc />
    public override void Dispose()
    {
        if (!_IsDisposed)
        {
            _IsDisposed = true;
            Log.LogInformation("Dispose");
            if (_SendTask.Status == TaskStatus.Running)
            {
                _SendTask.Wait();
            }
        }

        base.Dispose();
    }
}