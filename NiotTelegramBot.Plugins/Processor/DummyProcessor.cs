using System.Diagnostics;

namespace NiotTelegramBot.Plugins.Processor;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class DummyProcessor : IPluginProcessor
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<DummyProcessor> Log;

    /// <inheritdoc />
    public string Name => nameof(DummyProcessor);

    /// <inheritdoc />
    public Emoji Icon { get; set; } = Emoji.Robot;

    /// <inheritdoc />
    public string NameForUser { get; set; } = i18n.DummyProcessor;

    /// <inheritdoc />
    public TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    /// <inheritdoc />
    public bool Enabled { get; set; } = true;

    /// <inheritdoc />
    public SourceProcessors SourceProcessor { get; }

    /// <inheritdoc />
    public int Order { get; set; }

    /// <inheritdoc />
    public bool Healthcheck()
    {
        return true;
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Process(MessageProcess message)
    {
        Debug.Assert(Log != null);
        return Task.FromResult(new ProcessorResponseValue());
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Tick()
    {
        return Task.FromResult(new ProcessorResponseValue());
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public DummyProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        Log = loggerFactory.CreateLogger<DummyProcessor>();
        SourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Order = settings.Order;

        Log.LogInformation("Status: {Status}", Enabled ? Constants.STARTED : Constants.STAY_SLEPPING);
    }
}