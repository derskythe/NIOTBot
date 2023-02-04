using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;
using NiotTelegramBot.ModelzAndUtils.Settings;
using NiotTelegramBot.Plugins;
using NiotTelegramBot.Plugins.Processor.Abstract;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable once CheckNamespace
namespace Plugins.Processor;

public sealed class UserAuditProcessor : AbstractMessageTypeProcessor
{
    public new string Name => nameof(RuntimeErrorProcessor);
    
    /// <inheritdoc cref="IPluginProcessor" />
    public new Emoji Icon { get; set; } = Emoji.Robot;

    /// <inheritdoc cref="IPluginProcessor" />
    public new string NameForUser { get; set; } = i18n.UserAuditProcessor;

    /// <inheritdoc cref="IPluginProcessor" />
    public new TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public UserAuditProcessor(
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
        Permissions = UsersPermissions.System;
        SourceSourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        EventType = new[]
        {
            ProcessorEventType.AddUser, 
            ProcessorEventType.RemoveUser
        };
        MessageTitle = i18n.InfoUserAudit;
        AddEventType = true;

        log.LogInformation("Status: {Status}", Enabled ? Constants.STARTED : Constants.STAY_SLEPPING);
    }
}