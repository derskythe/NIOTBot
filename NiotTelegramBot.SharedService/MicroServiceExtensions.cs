using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NiotTelegramBot.SharedService;

/// <summary>
/// The extensions class
/// </summary>
public static class MicroServiceExtensions
{
    /// <summary>
    /// Adds AddApplicationInsightsTelemetryWorkerService, 
    /// set service grace shutdown timeout to shutdownDelay. Default: 10 sec
    /// and change BackgroundServiceExceptionBehavior to Ignore
    /// https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/hosting-exception-handling
    /// </summary>
    /// <param name="services">The services</param>
    /// <param name="shutdownDelay">The shutdown delay</param>
    /// <returns>The service collection</returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddBackgroundServiceBehavior(this IServiceCollection services, int shutdownDelay = 10)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        return services.Configure<HostOptions>(hostOptions =>
        {
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            hostOptions.ShutdownTimeout = TimeSpan.FromSeconds(shutdownDelay);
        });
    }

    /// <summary>
    /// Logs message with Current root path, .NET and operation system version
    /// Also add ActivityIdFormat.W3C and Activity.ForceDefaultIdFormat
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="microServiceName">Call MicroServiceHost.MicroServiceName and pass here</param>
    public static void LogInitMessage(this NLog.Logger logger, string microServiceName)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
#if DEBUG
        const string buildConfiguration = "DEBUG";
#else
        const string buildConfiguration = "RELEASE";
#endif
        logger.Warn("Init {MicroServiceName}:{BuildNumber} Git:{GitInfo} in {GetCurrentRootPath}, .NET: {Version}, OS: {OsVersion}, Build: {Build}",
                    microServiceName,
                    MicroServiceHost.BuildNumber,
                    MicroServiceHost.GitInfo,
                    MicroServiceHost.GetCurrentRootPath(),
                    Environment.Version,
                    Environment.OSVersion,
                    buildConfiguration
                   );
    }
}