using NiotTelegramBot.Plugins;
using NiotTelegramBot.Plugins.Processor.Abstract;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable once CheckNamespace
namespace NiotTelegramBot.Plugins.Processor;

public sealed class UserAuditProcessor : AbstractMessageTypeProcessor, IPluginProcessor
{
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
        var log = loggerFactory.CreateLogger<UserAuditProcessor>();

        Name = nameof(UserAuditProcessor);
        Icon = Emoji.BustsInSilhouette; // 👥
        NameForUser = i18n.UserAuditProcessor;
        
        // Set values to correct work
        Permissions = UsersPermissions.System;
        SourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
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