using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;
using NiotTelegramBot.ModelzAndUtils.Settings;
using NiotTelegramBot.Plugins;
using NiotTelegramBot.Plugins.Processor.Abstract;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace Plugins.Processor;

public sealed class RuntimeErrorProcessor : AbstractMessageTypeProcessor
{
    public new string Name => nameof(RuntimeErrorProcessor);

    /// <inheritdoc cref="IPluginProcessor" />
    public new Emoji Icon { get; set; } = Emoji.Fire;

    /// <inheritdoc cref="IPluginProcessor" />
    public new string NameForUser { get; set; } = i18n.RuntimeErrorProcessor;

    /// <inheritdoc cref="IPluginProcessor" />
    public new TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public RuntimeErrorProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
        : base(loggerFactory, settings, dataSources, chatUsers, cache, inputSettings, cancellationToken)
    {
        var log = loggerFactory.CreateLogger<RuntimeErrorProcessor>();

        // Set values to correct work
        SourceSourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Permissions = UsersPermissions.System;
        EventType = new[]
        {
            ProcessorEventType.RuntimeError
        };
        MessageTitle = i18n.ErrorRuntime;
        AddEventType = false;
        Order = settings.Order;

        log.LogInformation("Status: {Status}", Enabled ? Constants.STARTED : Constants.STAY_SLEPPING);
    }
}