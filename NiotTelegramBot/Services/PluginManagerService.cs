using System.Text;
using ModelzAndUtils.Enums;
using ModelzAndUtils.Interfaces;
using ModelzAndUtils.Models;
using ModelzAndUtils.Settings;
using Plugins.Processor;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NiotTelegramBot.Services;

/// <inheritdoc />
public class PluginManagerService : BackgroundService
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<PluginManagerService> Log;

    private bool _IsDisposed;
    private readonly CancellationTokenSource _Cts;

    //private Dictionary<string, IPluginInput> _PluginInput;
    private readonly IReadOnlyDictionary<string, IPluginDataSource> _PluginDataSource;
    private readonly Lazy<Dictionary<string, IPluginProcessor>> _PluginProcessor;
    private readonly Lazy<TelegramMenu[]> _MenuRoot;
    private readonly IMessageQueueService _MessageQueue;
    private readonly ICacheService _Cache;
    private readonly IChatUsers _ChatUsers;

    private Task _TickTask = Task.CompletedTask;
    private Task _ProcessTask = Task.CompletedTask;

    private readonly TimeSpan _TimeoutTick = TimeSpan.FromSeconds(60);
    private readonly TimeSpan _TimeoutProcessExecution = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _TimeoutDelayNextTick = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _TimeoutDelayNextProcess = TimeSpan.FromMilliseconds(100);

    #region Constructor and helpers

    public PluginManagerService(
        ILoggerFactory loggerFactory,
        IOptions<PluginInputArraySettings> configPluginInput,
        IOptions<PluginDataSourceArraySettings> configPluginDataSource,
        IOptions<PluginProcessorArraySettings> configPluginProcessor,
        IChatUsers chatUsers,
        IMessageQueueService messageQueue,
        ICacheService cache)
    {
        _ChatUsers = chatUsers;
        _MessageQueue = messageQueue;
        _Cache = cache;
        Log = loggerFactory.CreateLogger<PluginManagerService>();
        _Cts = new CancellationTokenSource();

        var configPluginOutgoingInput = configPluginInput.Value.List;
        //_PluginInput = configPluginOutgoingInput.ToDictionary(i => i.Name);
        if (configPluginOutgoingInput.Count == 0)
        {
            Log.LogWarning("Input plugin list is empty!");
        }

        if (configPluginDataSource.Value.List.Count == 0)
        {
            Log.LogWarning("Plugin DataSource list is empty!");
        }

        _PluginDataSource = InitDataSources(loggerFactory, configPluginDataSource, _Cts.Token);

        Dictionary<MessageType, PluginOutgoingInputSettings> existInput = configPluginOutgoingInput.Where(v => v.Name.IsEqual(
             new[]
             {
                 MessageType.Photo.AsString(),
                 MessageType.Text.AsString()
             }))
            .ToDictionary(v => Enums.Parse<MessageType>(v.Name));

        if (configPluginProcessor.Value.List.Count == 0)
        {
            Log.LogWarning("Plugin processor list is empty! Using dummy echo plugin");
            _PluginProcessor = new Lazy<Dictionary<string, IPluginProcessor>>(() => new Dictionary<string, IPluginProcessor>(2)
            {
                {
                    nameof(EchoProcessor), CreateProcessor(loggerFactory,
                                                           new PluginProcessorSettings()
                                                           {
                                                               Enabled = true
                                                           },
                                                           _PluginDataSource,
                                                           _ChatUsers,
                                                           _Cache,
                                                           existInput,
                                                           _Cts.Token)
                },
                {
                    nameof(FileProcessor), CreateProcessor(loggerFactory,
                                                           new PluginProcessorSettings()
                                                           {
                                                               Enabled = true
                                                           },
                                                           _PluginDataSource,
                                                           _ChatUsers,
                                                           _Cache,
                                                           existInput,
                                                           _Cts.Token)
                },
                {
                    nameof(DefaultMessagesProcessor),
                    CreateProcessor(loggerFactory,
                                    new PluginProcessorSettings()
                                    {
                                        Enabled = true,
                                        Name = nameof(DefaultMessagesProcessor)
                                    },
                                    _PluginDataSource,
                                    _ChatUsers,
                                    _Cache,
                                    existInput,
                                    _Cts.Token)
                }
            });
        }
        else
        {
            _PluginProcessor = new Lazy<Dictionary<string, IPluginProcessor>>(() =>
            {
                var list = configPluginProcessor.Value.List.ToDictionary(processor => processor.Name,
                                                                         processor => CreateProcessor(loggerFactory,
                                                                          processor,
                                                                          _PluginDataSource,
                                                                          _ChatUsers,
                                                                          _Cache,
                                                                          existInput,
                                                                          _Cts.Token));

                list.Add(nameof(DefaultMessagesProcessor),
                         CreateProcessor(loggerFactory,
                                         new PluginProcessorSettings()
                                         {
                                             Enabled = true,
                                             Name = nameof(DefaultMessagesProcessor)
                                         },
                                         _PluginDataSource,
                                         _ChatUsers,
                                         _Cache,
                                         existInput,
                                         _Cts.Token)
                        );

                return list;
            });
        }

        _MenuRoot = new Lazy<TelegramMenu[]>(() =>
        {
            var list = new List<TelegramMenu>();

            foreach (var (key, processor) in _PluginProcessor.Value)
            {
                if (!processor.Enabled ||
                    processor.SourceSourceProcessor is SourceProcessors.None or SourceProcessors.DummyProcessor)
                {
                    continue;
                }

                list.Add(new TelegramMenu(processor.Icon,
                                          processor.NameForUser,
                                          key,
                                          processor.Order,
                                          processor.SourceSourceProcessor));
            }

            var orderedList = list.OrderBy(i => i.Order).ToArray();
            Log.LogInformation("Root Menu:\n{Menu}",
                               orderedList.GetStringFromArray());
            return orderedList;
        });
    }

    private IPluginProcessor CreateProcessor(
        ILoggerFactory loggerFactory,
        PluginProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, PluginOutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        // ReSharper disable once CollectionNeverQueried.Local
        var constructorTypes = new Type[7];
        constructorTypes[0] = typeof(ILoggerFactory);
        constructorTypes[1] = typeof(PluginProcessorSettings);
        constructorTypes[2] = typeof(IReadOnlyDictionary<string, IPluginDataSource>);
        constructorTypes[3] = typeof(IChatUsers);
        constructorTypes[4] = typeof(ICacheService);
        constructorTypes[5] = typeof(IReadOnlyDictionary<MessageType, PluginOutgoingInputSettings>);
        constructorTypes[6] = typeof(CancellationToken);

        var type = Type.GetType(nameof(Plugins) + "." + settings.Name, false, true);
        if (type == null)
        {
            Log.LogError("Invalid Processor name: {Name}",
                         settings.Name);
        }
        else
        {
            try
            {
                var ctor = type.GetConstructor(constructorTypes) ??
                           throw new InvalidOperationException("GetConstructor problem");
                var instance = (IPluginProcessor)ctor.Invoke(new object[]
                               {
                                   loggerFactory,
                                   settings,
                                   dataSources,
                                   chatUsers,
                                   cache,
                                   inputSettings,
                                   cancellationToken
                               }) ??
                               throw new InvalidOperationException("Invoke constructor problem");
                if (!instance.Enabled && settings.Enabled)
                {
                    Log.LogWarning("Service {Name} init failed in constructor",
                                   instance.Name);
                }

                if (instance.SourceSourceProcessor.AsString() != settings.Name)
                {
                    Log.LogWarning("Invalid source processor {SourceProcessor} obtained by Service {Name}",
                                   instance.SourceSourceProcessor.AsString(),
                                   instance.Name);
                }

                return instance;
            }
            catch (Exception exp)
            {
                Log.LogError("Can't create Processor: {Name}, Error: {Message}", settings.Name, exp.Message);
            }
        }


        return new DummyProcessor(loggerFactory,
                                  settings,
                                  dataSources,
                                  chatUsers,
                                  cache,
                                  inputSettings,
                                  cancellationToken);
    }

    private Dictionary<string, IPluginDataSource> InitDataSources(
        ILoggerFactory loggerFactory,
        IOptions<PluginDataSourceArraySettings> config,
        CancellationToken cancellationToken)
    {
        var result = new List<IPluginDataSource>();
        var constructorTypes = new Type[3];
        constructorTypes[0] = typeof(ILoggerFactory);
        constructorTypes[1] = typeof(PluginDataSourceSettings);
        constructorTypes[2] = typeof(CancellationToken);

        foreach (var item in config.Value.List.Where(i => i.Enabled).ToList())
        {
            var type = Type.GetType(nameof(Plugins) + "." + item.Name, false, true);
            if (type == null)
            {
                var expMessage = $"Invalid DataSource name: {item.Name}";
                Log.LogError("{Name}", expMessage);
                AddErrorMessage(expMessage);
                continue;
            }

            try
            {
                var ctor = type.GetConstructor(constructorTypes) ?? throw new InvalidOperationException("GetConstructor problem");
                result.Add((IPluginDataSource)ctor.Invoke(new object[]
                           {
                               loggerFactory, item, cancellationToken
                           }) ??
                           throw new InvalidOperationException("Invoke constructor problem"));
            }
            catch (Exception exp)
            {
                var message = $"Can't create DataSource: {item.Name}, Error: {exp.Message}";
                AddErrorMessage(message);
                Log.LogError("{Message}", message);
            }
        }

        return result.Count > 0 ?
                   result.ToDictionary(i => i.Name) :
                   new Dictionary<string, IPluginDataSource>();
    }

    #endregion

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _TickTask = TickAsync(_Cts.Token);
        _ProcessTask = ProcessAsync(_Cts.Token);

        return Task.CompletedTask;
    }

    #region TickAsync

    private async Task TickAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var processor in _PluginProcessor.Value.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
            {
                try
                {
                    if (!processor.Value.Enabled)
                    {
                        continue;
                    }

                    // Create child cancelation token source to control too slow execution
                    using var childCts = CancellationTokenSource.CreateLinkedTokenSource(_Cts.Token);
                    childCts.CancelAfter(_TimeoutTick);

                    try
                    {
                        var response = await processor.Value.Tick();
                        if (response.IsException)
                        {
                            // We can send message to system users, but it's spam.
                            // So nothing to do
                            Log.LogWarning("Tick exception occured for {Key}. Message: {Message}",
                                           processor.Key,
                                           response.Message);
                        }

                        if (response.Responses != null && response.Responses.Count > 0)
                        {
                            _MessageQueue.OutgoingEnqueue(response.Responses);
                        }
                    }
                    catch (TaskCanceledException) when (childCts.IsCancellationRequested)
                    {
                        var errorMessage = $"Tick timeout ({_TimeoutTick.Seconds} sec) occured for {processor.Key}, processor was disabled";
                        Log.LogError("{Message}", errorMessage);
                        processor.Value.Enabled = false;
                        AddErrorMessage(errorMessage);
                    }
                }
                catch (TaskCanceledException exp)
                {
                    Log.LogInformation("{Message}", exp.Message);
                }
                catch (Exception exp)
                {
                    Log.LogError(exp, "Processor {Name} tick failed: {Message}", processor.Key, exp.Message);
                }
            }

            await Task.Delay(_TimeoutDelayNextTick, cancellationToken);
        }
    }

    #endregion

    private async Task ProcessAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = _MessageQueue.ProcessDequeue();
            if (message != null)
            {
                message.TryCount++;
                if (message.IncomingMessageChatId > 0)
                {
                    _MessageQueue.OutgoingEnqueue(new OutgoingMessage(message.IncomingMessageChatId,
                                                                      OutgoingMessageType.Typing,
                                                                      SourceProcessors.None));
                }
                else
                {
                    _MessageQueue.OutgoingEnqueue(new OutgoingMessage(OutgoingMessageType.Typing,
                                                                      UsersPermissions.Read,
                                                                      SourceProcessors.None));
                }

                if (!await ExecuteAllProcesses(message, cancellationToken))
                {
                    // We didn't get response, so we can set this message to default processor
                    message.IsOrphained = true;

                    if (message.TryCount > 5)
                    {
                        // We getting too many errors on this message
                        var msgCritical = $"Message {message} can't be processed! Remove message from queue";
                        Log.LogCritical("{Message}",
                                        msgCritical);
                        AddErrorMessage(msgCritical);
                    }
                    else
                    {
                        _MessageQueue.ProcessEnqueue(message);
                    }
                    //continue; // not wait anything
                }
                // else
                // {
                //     // Job is done
                // }
                // else
                // {
                //     var msgCritical = $"Could not obtain any result for message: {message}";
                //     Log.LogWarning("{Message}", msgCritical);                
                // }
                // TODO: disable user status "writing"
            }

            await Task.Delay(_TimeoutDelayNextProcess, cancellationToken);
        }
    }

    private async Task<bool> ExecuteAllProcesses(MessageProcess messageProcess, CancellationToken cancellationToken)
    {
        if (messageProcess.Type == ProcessorEventType.Unknown)
        {
            return true; // to not run on unknown messages, but deleted
        }

        foreach (var (key, pluginProcessor) in _PluginProcessor.Value)
        {
            // Only if cancelation occured we need to send messages to user
            if (messageProcess.Type == ProcessorEventType.Message && cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                if (!pluginProcessor.Enabled)
                {
                    continue;
                }

                using var childCts = CancellationTokenSource.CreateLinkedTokenSource(_Cts.Token);
                childCts.CancelAfter(_TimeoutProcessExecution);

                // If our attempt timed out, catch so that our retry loop continues.
                // Note: because the token is linked, the parent token may have been
                // cancelled. We check this at the beginning of the while loop.
                try
                {
                    var response = await pluginProcessor.Process(messageProcess);

                    if (response.IsException)
                    {
                        // Processor exception occured, we need to inform system users
                        AddErrorMessage(response.Message);
                    }

                    if (response.Responses is not { Count: > 0 })
                    {
                        continue;
                    }

                    // This processor have something to say
                    foreach (var item in response.Responses)
                    {
                        // Add menu if needed
                        AddMenuButtons(item);
                        _MessageQueue.OutgoingEnqueue(response.Responses);
                    }

                    // Only 1 processor can return result
                    return true;
                }
                catch (TaskCanceledException) when (childCts.IsCancellationRequested)
                {
                    var errorMessage = new StringBuilder()
                                       .Append(
                                               $"Process timeout ({_TimeoutProcessExecution.Seconds} sec) occured for " +
                                               $"{key}, processor was disabled")
                                       .ToString();
                    Log.LogError("{Message}", errorMessage);
                    pluginProcessor.Enabled = false;
                    AddErrorMessage(errorMessage);
                }
            }
            catch (TaskCanceledException exp)
            {
                Log.LogWarning("{Message} in {Name}",
                               exp.Message,
                               key);
            }
            catch (Exception exp)
            {
                var formatedMessageText = $"Processor {key} processing failed: {exp.Message}";
                Log.LogError(exp, "{Message}", formatedMessageText);
                AddErrorMessage(formatedMessageText);
            }
        }

        return false;
    }

    private void AddMenuButtons(OutgoingMessage item)
    {
        if (item.Keyboard != null && item.Keyboard.Count != 0)
        {
            return;
        }

        var keyboard = new List<TelegramButton>(_MenuRoot.Value.Length);
        keyboard.AddRange(
                          _MenuRoot.Value.Select(
                                                 row => new TelegramButton(row.Name)));

        item.Keyboard = keyboard;
    }

    private void AddErrorMessage(string errorMessage)
    {
        if (_ChatUsers.ListUsersByPermission(UsersPermissions.System).Count <= 0)
        {
            return;
        }

        _MessageQueue.ProcessEnqueue(new MessageProcess(ProcessorEventType.RuntimeError,
                                                        errorMessage));
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        if (!_Cts.IsCancellationRequested)
        {
            _Cts.Cancel();
        }

        if (!_IsDisposed)
        {
            Log.LogInformation("Dispose");
            _IsDisposed = true;
            foreach (var dataSource in _PluginDataSource)
            {
                dataSource.Value.Dispose();
            }
        }

        _ProcessTask.Wait();
        _TickTask.Wait();

        base.Dispose();
    }
}