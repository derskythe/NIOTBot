using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Settings;
using NiotTelegramBot.Services;
using NiotTelegramBot.SharedService;
using NLog;
using Telegram.Bot;

namespace NiotTelegramBot;

/// <summary>
/// The program class
/// </summary>
internal static class Program
{
    /// <summary>
    /// Main the args
    /// </summary>
    /// <param name="args">The args</param>
    private static async Task Main(string[] args)
    {
        var pathNlogConfig = Path.Combine(MicroServiceHost.GetCurrentRootPath(), "data", "config", "NLog.config");
        if (!File.Exists(pathNlogConfig))
        {
            File.Copy(Path.Combine(MicroServiceHost.GetCurrentRootPath(), "NLog.config"), pathNlogConfig);
        }

        var pathAppSettings = Path.Combine(MicroServiceHost.GetCurrentRootPath(), "data", "config", "appsettings.json");
        if (!File.Exists(pathAppSettings))
        {
            File.Copy(Path.Combine(MicroServiceHost.GetCurrentRootPath(),  "appsettings.json"), pathAppSettings);
        }

        var log = LogManager.LoadConfiguration(pathNlogConfig).GetCurrentClassLogger();
        try
        {
            log.LogInitMessage(MicroServiceHost.MicroServiceName);
            await MicroServiceHost.CreateConsoleHost(ConfigureServices, pathAppSettings, args).RunConsoleAsync();
        }
        catch (Exception exp)
        {
            log.Error(exp,
                      "Stopped program because of exception: {Message}",
                      exp.Message
                     );
            Thread.Sleep(5000);
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            LogManager.Shutdown();
        }
    }

    /// <summary>
    /// Configures the services using the specified host builder context
    /// </summary>
    /// <param name="hostBuilderContext">The host builder context</param>
    /// <param name="services">The services</param>
    private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
    {
        var configuration = hostBuilderContext.Configuration;
        services.AddOptions();
        var outgoingInputSection = configuration.GetSection(OutgoingInputSettings.NAME);
        services.AddOptions<List<OutgoingInputSettings>>()
                .Bind(outgoingInputSection);

        var dataSourceSection = configuration.GetSection(DataSourceSettings.NAME);
        services.AddOptions<List<DataSourceSettings>>()
                .Bind(dataSourceSection);

        var processorSection = configuration.GetSection(PluginProcessorArraySettings.NAME);
        services.AddOptions<List<ProcessorSettings>>()
                .Bind(processorSection);
        
        var sectionBot = configuration.GetSection(BotSettings.NAME);
        services.AddOptions<BotSettings>()
                .Bind(sectionBot);

        var apiKey = sectionBot.GetValue<string>("Token");

        services.AddHttpClient();
        // Services
        services.AddSingleton(configuration);
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
                                                                     _ => new TelegramBotClient(apiKey));
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IChatUsers, ChatUserService>();
        services.AddSingleton<IMessageQueueService, MessageQueueService>();

        // Background services
        services.AddBackgroundServiceBehavior();
        services.AddHostedService<ConsoleHostedService>();
        services.AddHostedService<PluginManagerService>();
        services.AddHostedService<BotService>();
    }
}