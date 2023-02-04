using ModelzAndUtils.Models;
using Plugins.Processor.Abstract;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable once CheckNamespace
namespace Plugins.Processor;

public sealed class StartStopInfoProcessor : AbstractMessageTypeProcessor
{
    public new string Name => nameof(StartStopInfoProcessor);
    
    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public new Emoji Icon { get; set; } = Emoji.Info; // ℹ

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public new string NameForUser { get; set; } = i18n.StartStopInfoProcessor;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public new TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public StartStopInfoProcessor(
        ILoggerFactory loggerFactory,
        PluginProcessorSettings processorSettings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, PluginOutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
        : base(loggerFactory, processorSettings, dataSources, chatUsers, cache, inputSettings, cancellationToken)
    {
        var log = loggerFactory.CreateLogger<RuntimeErrorProcessor>();

        // Set values to correct work
        SourceSourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Permissions = UsersPermissions.System;
        EventType = new[]
        {
            ProcessorEventType.BotStarted, 
            ProcessorEventType.BotStoped
        };
        MessageTitle = i18n.InfoBotStatusChanged;
        AddEventType = true;

        log.LogInformation("Status: {Status}", Enabled ? Constants.STARTED : Constants.STAY_SLEPPING);
    }
}