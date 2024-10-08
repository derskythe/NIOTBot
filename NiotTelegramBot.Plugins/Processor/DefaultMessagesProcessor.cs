﻿// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace NiotTelegramBot.Plugins.Processor;

// NiotTelegramBot.Plugins.Processor.DefaultMessagesProcessor
public class DefaultMessagesProcessor : IPluginProcessor
{
    public string Name => nameof(DefaultMessagesProcessor);

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public Emoji Icon { get; set; } = Emoji.WearyCat; // 🙀

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public string NameForUser { get; set; } = i18n.DefaultMessagesProcessor;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    /// <inheritdoc />
    public SourceProcessors SourceProcessor { get; }

    /// <inheritdoc />
    public bool Enabled { get; set; }

    // ReSharper disable once InconsistentNaming
    private readonly ILogger<DefaultMessagesProcessor> Log;

    // ReSharper disable once NotAccessedField.Local
    private readonly ICacheService _Cache;

    // ReSharper disable once NotAccessedField.Local
    private readonly IChatUsers _ChatUsers;
    private readonly Random _Random;

    /// <inheritdoc />
    public int Order { get; set; }

    /// <inheritdoc />
    public bool Healthcheck()
    {
        return true;
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Tick()
    {
        return Task.FromResult(new ProcessorResponseValue());
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Process(MessageProcess message)
    {
        // TODO: change to getting more things
        // In first run we don't know which processor can answer        
        if (!Enabled || !message.IsOrphained || message.Update == null || message.Update.Type != UpdateType.Message)
        {
            return Task.FromResult(new ProcessorResponseValue());
        }

        if (message.Update.Message is not { } incomingMessage)
        {
            Log.LogInformation("Invalid message type received: {MessageType}",
                               message.Update.Type.AsString());

            var chatId = message.Update.Message?.Chat.Id ?? 0;
            if (chatId > 0)
            {
                return Task.FromResult(
                                       ProcessorResponseValue.SingleOutgoingMessage(
                                                                                    new OutgoingMessage(
                                                                                     chatId,
                                                                                     Emoji.Robot.MessageCombine(GetMessage()),
                                                                                     SourceProcessor,
                                                                                     Icon)
                                                                                   )
                                      );
            }

            var expMessage = $"Failed to obtain response chat ID! Proccess: {message}";
            Log.LogError("{Message}", expMessage);
            return Task.FromResult(new ProcessorResponseValue(expMessage));
        }

        // if (incomingMessage.Text is not { } messageText)
        // {
        //     Log.LogWarning("Invalid message received");
        //     return Task.FromResult(new ProcessorResponseValue(new List<OutgoingMessage>()
        //     {
        //         new(
        //             MessageType.Text,
        //             Emoji.Warning.MessageCombine(i18n.ErrorInvalidMessage))
        //     }));
        // }

        var messageText = incomingMessage.Type.MessageShort(incomingMessage.Text);
        // var responseMessage = new StringBuilder();
        // responseMessage.Append($"{i18n.MessageReceived} {messageText[..]}");
        Log.LogInformation("{Text}", messageText);
        return Task.FromResult(
                               ProcessorResponseValue.SingleOutgoingMessage(
                                                                            new OutgoingMessage(
                                                                             incomingMessage.Chat.Id,
                                                                             Emoji.Robot.MessageCombine(GetMessage()),
                                                                             SourceProcessor,
                                                                             Icon)
                                                                           )
                              );
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public DefaultMessagesProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        _Cache = cache;
        _ChatUsers = chatUsers;

        Log = loggerFactory.CreateLogger<DefaultMessagesProcessor>();
        SourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Enabled = true;
        Order = settings.Order;

        Log.LogInformation("Status: {Status}",
                           Enabled ?
                               Constants.STARTED :
                               Constants.STAY_SLEPPING);
        _Random = new Random((int)DateTime.Now.Ticks);
    }

    private string GetMessage()
    {
        return _Random.Next(0, 4) switch
        {
            0 => i18n.InfoAtYourCommand,
            1 => i18n.InfoAtYourService,
            2 => i18n.InfoAwaitingOrders,
            3 => i18n.InfoYourOrders,
            _ => i18n.InfoIcomeToServe
        };
    }
}