using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace NiotTelegramBot.Plugins.Processor;

// ReSharper disable once UnusedType.Global
public class DockerProcessor : IPluginProcessor
{
    // ReSharper disable once InconsistentNaming
    private readonly ILogger<DockerProcessor> Log;

    /// <inheritdoc />
    public string Name => nameof(DockerProcessor);

    /// <inheritdoc />
    public Emoji Icon { get; set; } = Emoji.SpoutingWhale;

    private string IconString => Icon.AsString(EnumFormat.DisplayName) ??
                                 throw new InvalidOperationException($"Can't convert icon {Icon.AsString()}");

    /// <inheritdoc />
    public string NameForUser { get; set; } = i18n.DockerProcessor;

    /// <inheritdoc />
    public TelegramMenu[] Menu { get; set; } =
    {
        new(
            Emoji.Back,
            i18n.Back,
            0,
            SourceProcessors.DefaultMessagesProcessor)
    };

    /// <inheritdoc />
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public SourceProcessors SourceProcessor { get; }

    /// <inheritdoc />
    public int Order { get; set; }

    private readonly CancellationToken _CancellationToken;
    private readonly ICacheService _Cache;
    private readonly IChatUsers _ChatUsers;
    private readonly DockerClient _DockerClient;
    private readonly DockerClientConfiguration _DockerClientConfiguration;

    /// <inheritdoc />
    public bool Healthcheck()
    {
        return true;
    }

    /// <inheritdoc />
    public async Task<ProcessorResponseValue> Process(MessageProcess message)
    {
        // In first run we don't know which processor can answer        
        if (!Enabled || message.Update == null)
        {
            return new ProcessorResponseValue();
        }

        if (message.Update.Message is not { } incomingMessage)
        {
            return new ProcessorResponseValue();
        }

        var cache = _Cache.GetStringCache(CacheKeys.LatestAction);
        if (!string.IsNullOrEmpty(incomingMessage.Text) &&
            !incomingMessage.Text.StartsWith(IconString) &&
            !cache.ExistsInCache)
        {
            return new ProcessorResponseValue();
        }

        var username = _ChatUsers.GetByChatId(incomingMessage.Chat.Id);
        if (username == null)
        {
            Log.LogWarning("User is null for chatID: {ChatID}, Chat username: {Username}",
                           incomingMessage.Chat.Id,
                           incomingMessage.Chat.Username);
            return new ProcessorResponseValue();
        }

        // Set mode for next commands
        _Cache.SetStringCache(
                              IconString,
                              CacheKeys.LatestAction,
                              username.Username);

        if (!cache.ExistsInCache)
        {
            return await ListContainers(username.ChatId, incomingMessage.MessageId);
        }

        return new ProcessorResponseValue();
    }

    private async Task<ProcessorResponseValue> ListContainers(long chatId, int messageId)
    {
        try
        {
            var version = await _DockerClient.System.GetVersionAsync(_CancellationToken);

            IList<ContainerListResponse> containerList = await _DockerClient.Containers.ListContainersAsync(
                                                              new ContainersListParameters
                                                              {
                                                                  // Filters = new Dictionary<string, IDictionary<string, bool>>
                                                                  // {
                                                                  //     
                                                                  // },
                                                                  All = true
                                                              },
                                                              _CancellationToken
                                                             );

            var keyboard = new List<TelegramButton>();
            foreach (var container in containerList)
            {
                var name = container.Names.GetStringFromArray();
                name = name.StartsWith('/') ? name[1..] : name;
                var id = container.ID.GetHashCode();
                var text = $"{name} {ConvertStatus(container.State)} {container.Status}";
                keyboard.Add(new TelegramButton(text, $"{id}"));
            }

            var outgoingMessage = new OutgoingMessage(chatId, $"Version: {version.Version}", SourceProcessor, messageId)
            {
                Keyboard = keyboard,
                InlineKeyboardPrefix = string.Empty, //$"{IconString};{chatId}",
                IsKeyboardInline = true,
                KeyboardPerRow = 1
            };

            return new ProcessorResponseValue(new List<OutgoingMessage>()
            {
                outgoingMessage
            });
        }
        catch (Exception exp)
        {
            Log.LogError(exp, "{Message}", exp.Message);
            return new ProcessorResponseValue()
            {
                Message = exp.Message,
                IsException = true
            };
        }
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Tick()
    {
        return Task.FromResult(new ProcessorResponseValue());
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public DockerProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        Enabled = false;
        _ChatUsers = chatUsers;
        _Cache = cache;
        _CancellationToken = cancellationToken;
        Log = loggerFactory.CreateLogger<DockerProcessor>();
        SourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Order = settings.Order;

        // Docker client
        _DockerClientConfiguration = new DockerClientConfiguration(new Uri(settings.Options));
        _DockerClient = _DockerClientConfiguration.CreateClient();

        Enabled = settings.Enabled;
        Log.LogInformation("Status: {Status}",
                           Enabled ?
                               Constants.STARTED :
                               Constants.STAY_SLEPPING);
    }

    private static string ConvertStatus(string status)
    {
        if (status.IsEqual("running"))
        {
            return Emoji.PlayButton.GetEmoji() ?? status;
        }

        return Emoji.StopButton.GetEmoji() ?? status;
    }
}