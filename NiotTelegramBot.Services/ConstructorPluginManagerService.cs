using System.Reflection;
using System.Text;
using EnumsNET;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;
using NiotTelegramBot.ModelzAndUtils.Settings;
using NiotTelegramBot.Plugins.Processor;
using Telegram.Bot.Types.Enums;

namespace NiotTelegramBot.Services;

public partial class PluginManagerService
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
    private readonly Lazy<IReadOnlyDictionary<string, IPluginProcessor>> _PluginProcessor;
    private readonly Lazy<IReadOnlyList<TelegramMenu>> _MenuRoot;
    private readonly IMessageQueueService _MessageQueue;
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
        IOptions<List<OutgoingInputSettings>> configPluginInput,
        IOptions<List<DataSourceSettings>> configPluginDataSource,
        IOptions<List<ProcessorSettings>> configPluginProcessor,
        IChatUsers chatUsers,
        IMessageQueueService messageQueue,
        ICacheService cache)
    {
        _ChatUsers = chatUsers;
        _MessageQueue = messageQueue;
        var cacheService = cache;
        Log = loggerFactory.CreateLogger<PluginManagerService>();
        _Cts = new CancellationTokenSource();

        const string assemblyName = "NiotTelegramBot.Plugins";
        var applicationBase = AppContext.BaseDirectory;
        var assembly = Assembly.LoadFrom(Path.Combine(applicationBase, assemblyName + ".dll"));

        var configPluginOutgoingInput = configPluginInput.Value;
        //_PluginInput = configPluginOutgoingInput.ToDictionary(i => i.Name);
        if (configPluginOutgoingInput.Count == 0)
        {
            Log.LogWarning("Outgoing input plugin list is empty!");
        }
        else
        {
            var str = new StringBuilder();
            foreach (var item in configPluginOutgoingInput)
            {
                str.AppendLine($"Name: {item.Name}, Enable: {item.Enabled}");
            }

            Log.LogInformation("OutgoingInput List:\n{List}",
                               str.ToString());
        }

        var dataSourceList = configPluginDataSource.Value;
        if (dataSourceList.Count == 0)
        {
            Log.LogWarning("DataSource list is empty!");
        }
        else
        {
            var str = new StringBuilder();
            foreach (var item in dataSourceList)
            {
                str.AppendLine($"Name: {item}, Enable: {item.Enabled}");
            }

            Log.LogInformation("DataSource List:\n{List}",
                               str.ToString());
        }

        _PluginDataSource = InitDataSources(loggerFactory,
                                            configPluginDataSource.Value,
                                            assembly,
                                            _Cts.Token);
        var existInput = configPluginOutgoingInput.Where(v => v.Name.IsEqual(
                                                                             new[]
                                                                             {
                                                                                 MessageType.Photo.AsString(),
                                                                                 MessageType.Text.AsString()
                                                                             }))
                                                  .ToDictionary(v => Enums.Parse<MessageType>(v.Name));

        var processorsConfig = configPluginProcessor.Value;
        if (processorsConfig.Count == 0)
        {
            Log.LogWarning("Processor list is empty! Using: {Echo}, {File}, {Default}",
                           nameof(EchoProcessor),
                           nameof(FileProcessor),
                           nameof(DefaultMessagesProcessor));
            _PluginProcessor = new Lazy<IReadOnlyDictionary<string, IPluginProcessor>>(() =>
            {
                var result = new Dictionary<string, IPluginProcessor>(3)
                {
                    {
                        nameof(EchoProcessor), CreateProcessor(loggerFactory,
                                                               new ProcessorSettings()
                                                               {
                                                                   Enabled = true,
                                                                   Name = nameof(EchoProcessor)
                                                               },
                                                               _PluginDataSource,
                                                               _ChatUsers,
                                                               cacheService,
                                                               existInput,
                                                               assembly,
                                                               _Cts.Token)
                    },
                    // {
                    //     nameof(FileProcessor), CreateProcessor(loggerFactory,
                    //                                            new ProcessorSettings()
                    //                                            {
                    //                                                Enabled = true,
                    //                                                Name = nameof(FileProcessor)
                    //                                            },
                    //                                            _PluginDataSource,
                    //                                            _ChatUsers,
                    //                                            cacheService,
                    //                                            existInput,
                    //                                            assembly,
                    //                                            _Cts.Token)
                    // },
                    {
                        nameof(DefaultMessagesProcessor), CreateProcessor(loggerFactory,
                                                                          new ProcessorSettings()
                                                                          {
                                                                              Enabled = true,
                                                                              Name = nameof(DefaultMessagesProcessor)
                                                                          },
                                                                          _PluginDataSource,
                                                                          _ChatUsers,
                                                                          cacheService,
                                                                          existInput,
                                                                          assembly,
                                                                          _Cts.Token)
                    }
                };
                return result;
            });
        }
        else
        {
            _PluginProcessor = new Lazy<IReadOnlyDictionary<string, IPluginProcessor>>(() =>
            {
                var list = new Dictionary<string, IPluginProcessor>(processorsConfig.Count + 2);
                foreach (var processor in processorsConfig)
                {
                    if (string.IsNullOrEmpty(processor.Name))
                    {
                        Log.LogError("Invalid empty name for processor! Ignoring");
                        continue;
                    }
                    var name = processor.Name.EndsWith("Processor") ?
                                   processor.Name :
                                   processor.Name + "Processor";

                    if (list.ContainsKey(name))
                    {
                        Log.LogWarning("Second processor with same name: {Name}, ignoring", processor.Name);
                        continue;
                    }

                    list.Add(name, CreateProcessor(loggerFactory,
                                                   processor,
                                                   _PluginDataSource,
                                                   _ChatUsers,
                                                   cacheService,
                                                   existInput,
                                                   assembly,
                                                   _Cts.Token));
                }

                if (!list.ContainsKey(nameof(RuntimeErrorProcessor)))
                {
                    list.Add(nameof(RuntimeErrorProcessor),
                             CreateProcessor(loggerFactory,
                                             new ProcessorSettings()
                                             {
                                                 Enabled = true,
                                                 Name = nameof(RuntimeErrorProcessor)
                                             },
                                             _PluginDataSource,
                                             _ChatUsers,
                                             cacheService,
                                             existInput,
                                             assembly,
                                             _Cts.Token)
                            );
                }

                if (!list.ContainsKey(nameof(DefaultMessagesProcessor)))
                {
                    list.Add(nameof(DefaultMessagesProcessor),
                             CreateProcessor(loggerFactory,
                                             new ProcessorSettings()
                                             {
                                                 Enabled = true,
                                                 Name = nameof(DefaultMessagesProcessor)
                                             },
                                             _PluginDataSource,
                                             _ChatUsers,
                                             cacheService,
                                             existInput,
                                             assembly,
                                             _Cts.Token)
                            );
                }
                return list;
            });
        }

        _MenuRoot = new Lazy<IReadOnlyList<TelegramMenu>>(() =>
        {
            var dictionary = new Dictionary<string, TelegramMenu>();

            foreach (var (key, processor) in _PluginProcessor.Value)
            {
                if (!processor.Enabled ||
                    processor.Menu.Length == 0 ||
                    processor.SourceProcessor is SourceProcessors.None or SourceProcessors.DummyProcessor)
                {
                    continue;
                }

                var keyName = processor.Icon + processor.NameForUser;
                if (!dictionary.ContainsKey(keyName))
                {
                    dictionary.Add(keyName,
                                   new TelegramMenu(processor.Icon,
                                                    processor.NameForUser,
                                                    key,
                                                    processor.Order,
                                                    processor.SourceProcessor));
                }
            }

            var list = dictionary
                       .Select(c => c.Value)
                       .OrderBy(i => i.Order);
            var orderedList = list.ToArray();
            Log.LogInformation("Root Menu:\n{Menu}",
                               orderedList.GetStringFromArray());
            return orderedList;
        });
    }

    private IPluginProcessor CreateProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        Assembly assembly,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(settings.Name))
        {
            Log.LogWarning("Empty name of processor!");
            return new DummyProcessor(loggerFactory,
                                      new ProcessorSettings()
                                      {
                                          Name = Guid.NewGuid().ToString(),
                                          Enabled = false
                                      },
                                      dataSources,
                                      chatUsers,
                                      cache,
                                      inputSettings,
                                      cancellationToken);
        }

        if (!settings.Name.EndsWith("Processor"))
        {
            settings.Name += "Processor";
        }

        // ReSharper disable once CollectionNeverQueried.Local
        var constructorTypes = new Type[7];
        constructorTypes[0] = typeof(ILoggerFactory);
        constructorTypes[1] = typeof(ProcessorSettings);
        constructorTypes[2] = typeof(IReadOnlyDictionary<string, IPluginDataSource>);
        constructorTypes[3] = typeof(IChatUsers);
        constructorTypes[4] = typeof(ICacheService);
        constructorTypes[5] = typeof(IReadOnlyDictionary<MessageType, OutgoingInputSettings>);
        constructorTypes[6] = typeof(CancellationToken);

        var fullName = "NiotTelegramBot.Plugins.Processor." + settings.Name;
        var type = assembly.GetType(fullName, false, true);
        if (type == null)
        {
            Log.LogError("Invalid Processor name: {Name}", settings.Name);
        }
        else
        {
            try
            {
                const BindingFlags bindingFlags =
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                var ctor = type.GetConstructor(
                                               bindingFlags,
                                               null,
                                               CallingConventions.HasThis,
                                               constructorTypes,
                                               null) ??
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

                if (instance.SourceProcessor.AsString() != settings.Name)
                {
                    Log.LogWarning("Invalid source processor {SourceProcessor} obtained by Service {Name}",
                                   instance.SourceProcessor.AsString(),
                                   instance.Name);
                }

                Log.LogInformation("Processor Init: {Instance} ({Enabled})",
                                   instance.Name,
                                   instance.Enabled);
                return instance;
            }
            catch (Exception exp)
            {
                Log.LogError("Can't create Processor: {Name}, Error: {Message}", settings.Name, exp.Message);
            }
        }

        return new DummyProcessor(loggerFactory,
                                  new ProcessorSettings()
                                  {
                                      Name = settings.Name,
                                      Enabled = false
                                  },
                                  dataSources,
                                  chatUsers,
                                  cache,
                                  inputSettings,
                                  cancellationToken);
    }

    private Dictionary<string, IPluginDataSource> InitDataSources(
        ILoggerFactory loggerFactory,
        IReadOnlyList<DataSourceSettings> config,
        Assembly assembly,
        CancellationToken cancellationToken)
    {
        var result = new List<IPluginDataSource>();
        var constructorTypes = new Type[3];
        constructorTypes[0] = typeof(ILoggerFactory);
        constructorTypes[1] = typeof(DataSourceSettings);
        constructorTypes[2] = typeof(CancellationToken);

        foreach (var item in config.Where(i => i.Enabled).ToList())
        {
            var fullName = "NiotTelegramBot.Plugins.DataSource." + item.Name + "DataSource";
            var type = assembly.GetType(fullName, false, true);
            if (type == null)
            {
                var expMessage = $"Invalid DataSource name: {item.Name}";
                Log.LogError("{Name}", expMessage);
                AddErrorMessage(expMessage);
                continue;
            }

            try
            {
                const BindingFlags bindingFlags =
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                var ctor = type.GetConstructor(
                                               bindingFlags,
                                               null,
                                               CallingConventions.HasThis,
                                               constructorTypes,
                                               null) ??
                           throw new InvalidOperationException("GetConstructor problem");

                var instance = (IPluginDataSource)ctor.Invoke(new object[]
                               {
                                   loggerFactory,
                                   item,
                                   cancellationToken
                               }) ??
                               throw new InvalidOperationException("Invoke constructor problem");
                Log.LogInformation("Processor Init: {Instance} ({Enabled})",
                                   instance.Name,
                                   instance.Enabled);
                result.Add(instance);
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
}