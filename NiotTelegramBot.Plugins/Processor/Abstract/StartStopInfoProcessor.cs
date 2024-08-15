using NiotTelegramBot.Plugins.Processor.Abstract;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable once CheckNamespace
namespace NiotTelegramBot.Plugins.Processor;

public sealed class StartStopInfoProcessor : AbstractMessageTypeProcessor, IPluginProcessor
{
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public StartStopInfoProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
        : base(loggerFactory, settings, dataSources, chatUsers, cache, inputSettings, cancellationToken)
    {
        var log = loggerFactory.CreateLogger<StartStopInfoProcessor>();

        // Set values to correct work
        Name = nameof(StartStopInfoProcessor);
        Icon = Emoji.Info;

        SourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
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