using System.Text;

namespace NiotTelegramBot.Plugins.Processor;

/// <inheritdoc />
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class EchoProcessor : IPluginProcessor
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<EchoProcessor> Log;

    /// <inheritdoc />
    public Emoji Icon { get; set; } = Emoji.Robot;

    /// <inheritdoc />
    public string NameForUser { get; set; } = i18n.EchoProcessor;

    /// <inheritdoc />
    public TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    /// <inheritdoc />
    public string Name => nameof(EchoProcessor);

    /// <inheritdoc />
    public bool Enabled { get; set; } = true;

    /// <inheritdoc />
    public SourceProcessors SourceProcessor { get; }

    /// <inheritdoc />
    public int Order { get; set; }

    private bool _InvalidConfig;

    private readonly IReadOnlyDictionary<string, IPluginDataSource> _DataSources;

    /// <inheritdoc />
    public bool Healthcheck()
    {
        // if (!_NoUsers && (_ChatUsers.Users == null || _ChatUsers.Users.Count == 0))
        // {
        //     _NoUsers = true;
        //     Log.LogWarning("Chat users list is empty!");
        // }

        if (!_InvalidConfig && _DataSources.Count == 0)
        {
            _InvalidConfig = true;
            Log.LogError("DataSources is empty!");
        }

        return true;
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Process(MessageProcess message)
    {
        Log.LogDebug("Type: {Type}",
                     message.Type.AsString());
        if (!Enabled || message.Type != ProcessorEventType.Message || message.Update == null)
        {
            return Task.FromResult(new ProcessorResponseValue());
        }

        if (message.Update.Message is not { } incomingMessage)
        {
            Log.LogWarning("Invalid message type received: {MessageType}", message.Update.Type.AsString());
            return Task.FromResult(new ProcessorResponseValue());
        }

        if (incomingMessage.Text is not { } messageText)
        {
            Log.LogWarning("Invalid message received");
            return Task.FromResult(
                                   ProcessorResponseValue.SingleOutgoingMessage(
                                                                                new OutgoingMessage(incomingMessage.Chat.Id,
                                                                                 Emoji.Warning
                                                                                      .MessageCombine(i18n.ErrorInvalidMessage),
                                                                                 SourceProcessor,
                                                                                 Icon)
                                                                               ));
        }

        var responseMessage = new StringBuilder();
        responseMessage.Append($"{i18n.MessageReceived} {messageText.MessageShort()}");

        Log.LogInformation("{Text}", responseMessage);
        return Task.FromResult(
                               ProcessorResponseValue.SingleOutgoingMessage(
                                                                            new OutgoingMessage(
                                                                             incomingMessage.Chat.Id,
                                                                             Emoji.Robot.MessageCombine(responseMessage),
                                                                             SourceProcessor,
                                                                             Icon)
                                                                           )
                              );
    }

    public Task<ProcessorResponseValue> Tick()
    {
        return Task.FromResult(new ProcessorResponseValue());
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public EchoProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        Log = loggerFactory.CreateLogger<EchoProcessor>();
        _DataSources = dataSources;
        SourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Order = settings.Order;

        Log.LogInformation("Status: {Status}",
                           Enabled ?
                               Constants.STARTED :
                               Constants.STAY_SLEPPING);
    }
}