// ReSharper disable NotAccessedField.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

using System.Text;

namespace NiotTelegramBot.Plugins.Processor.Abstract;

// ReSharper disable once UnusedType.Global
public abstract class AbstractMessageTypeProcessor
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<AbstractMessageTypeProcessor> Log;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public Emoji Icon { get; set; } = Emoji.Robot;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public string NameForUser { get; set; } = i18n.AbstractMessageTypeProcessor;

    /// <inheritdoc cref="ModelzAndUtils.Interfaces.IPluginProcessor" />
    public TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    public string Name { get; protected set; } = nameof(AbstractMessageTypeProcessor);

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public SourceProcessors SourceProcessor { get; protected set; }

    public bool Enabled { get; set; }

    public int Order { get; set; }

    // ReSharper disable InconsistentNaming
    protected readonly IChatUsers _ChatUsers;
    protected readonly IReadOnlyDictionary<string, IPluginDataSource> _DataSources;
    protected readonly ProcessorSettings Settings;

    protected readonly CancellationToken _CancellationToken;

    protected readonly ICacheService _Cache;
    // ReSharper restore InconsistentNaming

    public bool Healthcheck()
    {
        return Settings.Enabled == Enabled;
    }

    public Task<ProcessorResponseValue> Tick()
    {
        return Task.FromResult(new ProcessorResponseValue());
    }

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
            Log.LogInformation("No users can be found for permission: {Permission}",
                               Permissions.AsString());
            return Task.FromResult(new ProcessorResponseValue());
        }

        var text = new StringBuilder();
        text.Append(MessageTitle).Append(' ');
        text.Append(AddEventType ?
                        $"{message.Type.AsString(EnumFormat.EnumMemberValue)} {message.AdditionalInfo}" :
                        message.AdditionalInfo);
        var buildedText = Icon.MessageCombine(text);
        Log.LogInformation("Text: {Text}", buildedText);
        var messageList = listAllowedUsers.Select(user => new OutgoingMessage(
                                                                              user.ChatId,
                                                                              OutgoingMessageType.Text,
                                                                              buildedText,
                                                                              SourceProcessor,
                                                                              Icon)
                                                 )
                                          .ToList();

        return Task.FromResult(new ProcessorResponseValue(messageList));
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
#pragma warning disable CS8618
    protected AbstractMessageTypeProcessor(
#pragma warning restore CS8618
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
        SourceProcessor = SourceProcessors.InvalidProcessor;
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