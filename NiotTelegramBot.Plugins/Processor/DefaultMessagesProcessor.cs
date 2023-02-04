// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;
using NiotTelegramBot.ModelzAndUtils.Settings;

namespace NiotTelegramBot.Plugins.Processor;

public class DefaultMessagesProcessor : IPluginProcessor
{
    public string Name => nameof(DefaultMessagesProcessor);

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public Emoji Icon { get; set; } = Emoji.WearyCat; // 🙀

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public string NameForUser { get; set; } = i18n.OrphainedMessagesProcessor;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    /// <inheritdoc />
    public SourceProcessors SourceSourceProcessor { get; }

    /// <inheritdoc />
    public bool Enabled { get; set; }

    // ReSharper disable once InconsistentNaming
    private readonly ILogger<DefaultMessagesProcessor> Log;
    
    private readonly ICacheService _Cache;

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
        if (!Enabled || message.Type != ProcessorEventType.Message || !message.IsOrphained || message.Update == null)
        {
            return Task.FromResult(new ProcessorResponseValue());
        }

        if (message.Update.Message is not { } incomingMessage)
        {
            Log.LogInformation("Invalid message type received: {MessageType}",
                               message.Update.Type.AsString());
            return Task.FromResult(new ProcessorResponseValue());
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

        var messageText = !string.IsNullOrEmpty(incomingMessage.Text) ?
                              incomingMessage.Text[..255] :
                              $"Type: {incomingMessage.Type.AsString()}";
        // var responseMessage = new StringBuilder();
        // responseMessage.Append($"{i18n.MessageReceived} {messageText[..250]}");
        Log.LogInformation("{Text}", messageText);
        return Task.FromResult(
                               ProcessorResponseValue.SingleOutgoingMessage(
                                                                            new OutgoingMessage(
                                                                             incomingMessage.Chat.Id,
                                                                             Emoji.Robot.MessageCombine(i18n.MessageSelectMenu),
                                                                             SourceSourceProcessor)
                                                                           )
                              );
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public DefaultMessagesProcessor(
        ILoggerFactory loggerFactory,
        PluginProcessorSettings processorSettings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, PluginOutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        _Cache = cache;
        Log = loggerFactory.CreateLogger<DefaultMessagesProcessor>();
        SourceSourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Enabled = true;
        Order = processorSettings.Order;

        Log.LogInformation("Status: {Status}", Enabled ? Constants.STARTED : Constants.STAY_SLEPPING);
    }
}