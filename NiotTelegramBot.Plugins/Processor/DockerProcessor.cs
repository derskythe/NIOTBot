using System.Runtime.Serialization;
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

        var incomingMessage = message.Update.Message;
        var callbackQuery = message.Update.CallbackQuery;
        if (incomingMessage == null && callbackQuery == null)
        {
            return new ProcessorResponseValue();
        }

        string text;
        string username;
        long chatId;
        int messageId;

        if (callbackQuery != null)
        {
            chatId = callbackQuery.Message?.Chat.Id ?? 0L;
            username = callbackQuery.Message?.Chat.Username ?? string.Empty;
            text = callbackQuery.Data ?? string.Empty;
            messageId = callbackQuery.Message?.MessageId ?? 0;
        }
        else if (incomingMessage != null)
        {
            chatId = incomingMessage.Chat.Id;
            username = incomingMessage.Chat.Username ?? string.Empty;
            text = incomingMessage.Text ?? string.Empty;
            messageId = incomingMessage.MessageId;
        }
        else
        {
            return new ProcessorResponseValue();
        }
        var cache = _Cache.GetStringCache(CacheKeys.LatestAction, username);
        var telegramUser = _ChatUsers.GetByChatId(chatId);

        if (!string.IsNullOrEmpty(text) &&
            !text.StartsWith(IconString) &&
            !cache.ExistsInCache)
        {
            return new ProcessorResponseValue();
        }

        if (telegramUser == null)
        {
            Log.LogWarning("User is null for chatID: {ChatID}, Chat username: {Username}",
                           chatId,
                           username);
            return new ProcessorResponseValue();
        }

        // Set mode for next commands
        _Cache.SetStringCache(
                              IconString,
                              CacheKeys.LatestAction,
                              telegramUser.Username);

        if (!cache.ExistsInCache)
        {
            return await ListContainers(telegramUser.ChatId, messageId);
        }
        else if (callbackQuery != null)
        {
            return await ProcessCallbackQuery(telegramUser.ChatId, text);
        }

        return new ProcessorResponseValue();
    }

    private async Task<ProcessorResponseValue> ProcessCallbackQuery(long chatId, string query)
    {
        try
        {
            // Type;ChatID;SubType;Prefix;Text
            var splited = query.Split(';');
            var subType = Enums.Parse<CallbackType>(splited[2]);

            if (subType == CallbackType.None)
            {
                return await ListContainers(chatId, 0);
            }
            if (subType == CallbackType.Info)
            {
                var id = splited[4];
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

                var selectedContainer = containerList.FirstOrDefault(c => c.ID.GetHashCode() == id.GetHashCode());

                if (selectedContainer == null)
                {
                    return new ProcessorResponseValue(new List<OutgoingMessage>()
                    {
                        new(chatId,
                            Emoji.Warning.MessageCombine("Invalid container"),
                            SourceProcessor,
                            Icon)
                    });
                }

                var name = GetName(selectedContainer.Names);
                var image = selectedContainer.Image;
                var ports = string.Join(' ',
                                        selectedContainer.Ports?.Select(p => $"{p.PublicPort}:${p.PrivatePort}") ?? Array.Empty<string>());
                var state = Enums.Parse<ContainerState>(selectedContainer.State, true);
                string buttonAction;
                string actionType;
                if (state == ContainerState.Running)
                {
                    buttonAction = i18n.InfoStop;
                    actionType = CallbackType.Stop.AsString();
                }
                else
                {
                    buttonAction = i18n.InfoRun;
                    actionType = CallbackType.Run.AsString();
                }

                var message = Emoji.Package.MessageCombine(true, $"{i18n.InfoName}: {name}",
                                                           $"{i18n.InfoImage}: {image}",
                                                           $"{i18n.InfoState}: {state.AsString()}",
                                                           $"{i18n.InfoPorts}: {ports}",
                                                           $"{i18n.InfoStarted}: {selectedContainer.Status}",
                                                           $"{i18n.InfoCommand}: {selectedContainer.Command}",
                                                           $"{i18n.InfoCreated}: {selectedContainer.Created:u}"
                                                          );
                var keyboard = new List<TelegramButton>
                {
                    new ($"{ConvertStatus(state)} {buttonAction}", $"{actionType};{id}"),
                    new (i18n.InfoLogs, $"{CallbackType.Logs.AsString()};{id}"),
                    new (i18n.InfoStats, $"{CallbackType.Stats.AsString()};{id}"),
                    new (i18n.InfoDelete, $"{CallbackType.Delete.AsString()};{id}")
                };
                var outgoingMessage = new OutgoingMessage(chatId,
                                                          message,
                                                          SourceProcessor,
                                                          Icon)
                {
                    Keyboard = keyboard,
                    InlineKeyboardPrefix = string.Empty,
                    IsKeyboardInline = true,
                    KeyboardPerRow = 2
                };
                return new ProcessorResponseValue(new List<OutgoingMessage>()
                {
                    outgoingMessage
                });
            }

            return new ProcessorResponseValue(new List<OutgoingMessage>()
            {
                new(chatId,
                    Emoji.Warning.MessageCombine("Invalid container"),
                    SourceProcessor,
                    Icon)
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
            var running = 0;
            var stopped = 0;
            var callbackType = CallbackType.Info.AsString();
            foreach (var container in containerList)
            {
                var name = GetName(container.Names);
                var id = container.ID.GetHashCode();
                var state = Enums.Parse<ContainerState>(container.State, true);

                if (state == ContainerState.Running)
                {
                    running++;
                }
                else
                {
                    stopped++;
                }

                var text = $"{name} {ConvertStatus(state)} {container.Status}";
                keyboard.Add(new TelegramButton(text, $"{callbackType};{id}"));
            }

            var messageText = Emoji.SpoutingWhale.MessageCombine(true,
                                                                 $"{i18n.InfoVersion}: {version.Version}",
                                                                 $"{i18n.InfoTotal}: {running + stopped},",
                                                                 $"{i18n.InfoRunning}: {running},",
                                                                 $"{i18n.InfoStoped}: {stopped}");
            var outgoingMessage = new OutgoingMessage(chatId,
                                                      messageText,
                                                      SourceProcessor,
                                                      Icon,
                                                      messageId)
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

    private static string GetName(IList<string>? containerNames)
    {
        var name = containerNames.GetStringFromArray();
        name = name.StartsWith('/') ? name[1..] : name;
        return name;
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

    private static string ConvertStatus(ContainerState status)
    {
        if (status == ContainerState.Running)
        {
            return Emoji.PlayButton.GetEmoji() ?? status.AsString();
        }

        return Emoji.StopButton.GetEmoji() ?? status.AsString();
    }
}
[Serializable]
public enum CallbackType
{
    [EnumMember] None,
    [EnumMember] Info,
    [EnumMember] Run,
    [EnumMember] Stop,
    [EnumMember] Logs,
    [EnumMember] Stats,
    [EnumMember] Delete,
}
[Serializable]
public enum ContainerState
{
    [EnumMember] Unknown,
    Running,
    [EnumMember] Created,
    [EnumMember] Stopped
}