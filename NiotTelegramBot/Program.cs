using Microsoft.Extensions.DependencyInjection;
using ModelzAndUtils.Interfaces;
using ModelzAndUtils.Settings;
using NiotTelegramBot.Services;
using NLog;
using SharedService;
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

        var log = LogManager.LoadConfiguration(pathNlogConfig).GetCurrentClassLogger();
        try
        {
            log.LogInitMessage(MicroServiceHost.MicroServiceName);
            await MicroServiceHost.CreateConsoleHost(ConfigureServices, args).RunConsoleAsync();
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
        var sectionPluginInput = configuration.GetSection(PluginInputArraySettings.NAME);
        services.AddOptions<IEnumerable<PluginInputArraySettings>>()
                .Bind(sectionPluginInput);

        var sectionPluginDataSource = configuration.GetSection(PluginDataSourceArraySettings.NAME);
        services.AddOptions<IEnumerable<PluginDataSourceArraySettings>>()
                .Bind(sectionPluginDataSource);

        var sectionPluginProcessor = configuration.GetSection(PluginProcessorArraySettings.NAME);
        services.AddOptions<IEnumerable<PluginProcessorArraySettings>>()
                .Bind(sectionPluginProcessor);

        var apiKey = configuration.GetSection("Token").Value ?? string.Empty;

        services.AddHttpClient();
        // Services
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
                                                                     _ => new TelegramBotClient(apiKey));
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IChatUsers, ChatUserService>();
        services.AddSingleton<IMessageQueueService, MessageQueueService>();

        // Background services
        services.AddBackgroundServiceBehavior();
        services.AddHostedService<ConsoleHostedService>();
        services.AddHostedService<PluginManagerService>();
    }
}