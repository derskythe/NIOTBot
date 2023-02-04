using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SharedService;

/// <summary>
/// The web hosted application lifetime service class
/// </summary>
/// <seealso cref="IHostedService"/>
public class WebHostedApplicationLifetimeService : IHostedService
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<WebHostedApplicationLifetimeService> Log;
    /// <summary>
    /// The application lifetime
    /// </summary>
    private readonly IHostApplicationLifetime _ApplicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebHostedApplicationLifetimeService"/> class
    /// </summary>
    /// <param name="applicationLifetime">The application lifetime</param>
    /// <param name="logger">The logger</param>
    public WebHostedApplicationLifetimeService(
        IHostApplicationLifetime applicationLifetime,
        ILogger<WebHostedApplicationLifetimeService> logger)
    {
        _ApplicationLifetime = applicationLifetime;
        Log = logger;
    }

    /// <summary>
    /// Starts the cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ApplicationLifetime.ApplicationStarted.Register(() => { OnStarted(cancellationToken); });
        _ApplicationLifetime.ApplicationStopping.Register(() => { OnStopping(cancellationToken); });
        _ApplicationLifetime.ApplicationStopped.Register(() => { OnStopped(cancellationToken); });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ons the started using the specified cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual void OnStarted(CancellationToken cancellationToken)
    {
        Log.LogInformation("ApplicationStarted");
    }

    /// <summary>
    /// Ons the stopped using the specified cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual void OnStopped(CancellationToken cancellationToken)
    {
        Log.LogInformation("Stopped!");
    }

    /// <summary>
    /// Ons the stopping using the specified cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual void OnStopping(CancellationToken cancellationToken)
    {
        Log.LogInformation("SIGTERM received, waiting for 10 seconds");
        Thread.Sleep(10_000);
        Log.LogInformation("Termination delay complete, continuing stopping process");
    }

    // Required to satisfy interface
    /// <summary>
    /// Stops the cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}