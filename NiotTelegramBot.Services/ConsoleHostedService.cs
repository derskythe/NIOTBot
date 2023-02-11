using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;

namespace NiotTelegramBot.Services;

/// <summary>
/// The console hosted service class
/// </summary>
/// <seealso cref="IHostedService"/>
public sealed class ConsoleHostedService : IHostedService
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<ConsoleHostedService> Log;

    /// <summary>
    /// The application lifetime
    /// </summary>
    private readonly IHostApplicationLifetime _ApplicationLifetime;

    private readonly IMessageQueueService _MessageQueueService;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> log,
        IHostApplicationLifetime applicationLifetime,
        IMessageQueueService messageQueueService)
    {
        Log = log;
        _ApplicationLifetime = applicationLifetime;
        _MessageQueueService = messageQueueService;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>Task.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var argsList = Environment.GetCommandLineArgs();
        if (argsList.Length > 0)
        {
            Log.LogInformation("Starting with arguments: {Args}", string.Join(" ", Environment.GetCommandLineArgs()));
        }

        // ApplicationStarted
        _ApplicationLifetime.ApplicationStarted.Register(() => { OnStarted(cancellationToken); });
        _ApplicationLifetime.ApplicationStopping.Register(() => { OnStopping(cancellationToken); });
        _ApplicationLifetime.ApplicationStopped.Register(() => { OnStopped(cancellationToken); });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ons the started using the specified cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    private void OnStarted(CancellationToken cancellationToken)
    {
        Log.LogInformation("ApplicationStarted");
        _MessageQueueService.ProcessEnqueue(new MessageProcess()
        {
            Type = ProcessorEventType.BotStarted
        });
        Task.Run(async () =>
                 {
                     while (!cancellationToken.IsCancellationRequested)
                     {
                         await Task.Delay(-1, cancellationToken);
                     }

                     _ApplicationLifetime.StopApplication();
                 },
                 cancellationToken
                );
    }

    /// <summary>
    /// Ons the stopped using the specified cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnStopped(CancellationToken cancellationToken)
    {
        Log.LogInformation("Stopped!");
    }

    /// <summary>
    /// Ons the stopping using the specified cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnStopping(CancellationToken cancellationToken)
    {
        Log.LogInformation("SIGTERM received, waiting for 3 seconds");
        _MessageQueueService.ProcessEnqueue(new MessageProcess()
        {
            Type = ProcessorEventType.BotStoped
        });
        Thread.Sleep(3_000);
        Log.LogInformation("Termination delay complete, continuing stopping process");
    }

    /// <summary>
    /// Stops the asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}