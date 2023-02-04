﻿// ReSharper disable NotAccessedField.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

using System.Text;
using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;
using NiotTelegramBot.ModelzAndUtils.Settings;

namespace NiotTelegramBot.Plugins.Processor.Abstract;

// ReSharper disable once UnusedType.Global
public abstract class AbstractMessageTypeProcessor : IPluginProcessor
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<AbstractMessageTypeProcessor> Log;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public virtual Emoji Icon { get; set; } = Emoji.Robot;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public virtual string NameForUser { get; set; } = i18n.AbstractMessageTypeProcessor;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public virtual TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    /// <inheritdoc />
    public virtual string Name => nameof(AbstractMessageTypeProcessor);

    /// <inheritdoc />
    public virtual SourceProcessors SourceSourceProcessor { get; protected set; }

    /// <inheritdoc />
    public virtual bool Enabled { get; set; }

    /// <inheritdoc />
    public int Order { get; set; }

    // ReSharper disable InconsistentNaming
    protected readonly IChatUsers _ChatUsers;
    protected readonly IReadOnlyDictionary<string, IPluginDataSource> _DataSources;
    protected readonly ProcessorSettings Settings;

    protected readonly CancellationToken _CancellationToken;

    protected readonly ICacheService _Cache;
    // ReSharper restore InconsistentNaming

    /// <inheritdoc />
    public bool Healthcheck()
    {
        return Settings.Enabled == Enabled;
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Tick()
    {
        return Task.FromResult(new ProcessorResponseValue());
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Process(MessageProcess message)
    {
        if (!Enabled || !EventType.Contains(message.Type))
        {
            return Task.FromResult(new ProcessorResponseValue());
        }

        Log.LogDebug("Received message {Type}:{Message}",
                     message.Type.AsString(),
                     message.AdditionalInfo);

        var listAllowedUsers = _ChatUsers.ListUsersByPermission(Permissions);
        if (listAllowedUsers.Count <= 0)
        {
            return Task.FromResult(new ProcessorResponseValue());
        }

        var text = new StringBuilder();
        text.Append(MessageTitle).Append(' ');
        text.Append(AddEventType ?
                        $"{message.Type.AsString()} {message.AdditionalInfo}" :
                        message.AdditionalInfo);
        var messageList = listAllowedUsers.Select(user => new OutgoingMessage(
                                                                              user.ChatId,
                                                                              OutgoingMessageType.Text,
                                                                              Icon.MessageCombine(text),
                                                                              SourceSourceProcessor)
                                                 )
                                          .ToList();

        return Task.FromResult(
                               new ProcessorResponseValue(
                                                          messageList));
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    protected AbstractMessageTypeProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        _Cache = cache;
        Log = loggerFactory.CreateLogger<AbstractMessageTypeProcessor>();
        _CancellationToken = cancellationToken;
        Settings = settings;
        _DataSources = dataSources;
        _ChatUsers = chatUsers;
        
        // ReSharper disable once VirtualMemberCallInConstructor
        Enabled = Settings.Enabled;
        // ReSharper disable once VirtualMemberCallInConstructor
        SourceSourceProcessor = SourceProcessors.InvalidProcessor;
        // ReSharper disable once VirtualMemberCallInConstructor
        Order = settings.Order;
    }

    protected UsersPermissions Permissions { get; set; } = UsersPermissions.None;

    protected ProcessorEventType[] EventType { get; set; } =
    {
        ProcessorEventType.NeverBelivied
    };

    protected string MessageTitle { get; set; } = string.Empty;

    protected bool AddEventType { get; set; }
}