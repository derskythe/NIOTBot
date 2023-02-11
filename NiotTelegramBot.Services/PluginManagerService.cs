using System.Text;
using EnumsNET;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Models;

namespace NiotTelegramBot.Services;

/// <inheritdoc />
public partial class PluginManagerService : BackgroundService
{
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
        Log.LogInformation("Task started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                foreach (var processor in _PluginProcessor.Value.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
                {
                    // Create child cancelation token source to control too slow execution
                    using var childCts = CancellationTokenSource.CreateLinkedTokenSource(_Cts.Token);
                    childCts.CancelAfter(_TimeoutTick);

                    try
                    {
                        if (!processor.Value.Enabled)
                        {
                            continue;
                        }

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
                            var errorMessage =
                                $"Tick timeout ({_TimeoutTick.Seconds} sec) occured for {processor.Key}, processor was disabled";
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
            }
            catch (TaskCanceledException exp)
            {
                Log.LogInformation("{Message}", exp.Message);
            }
            catch (Exception exp)
            {
                Log.LogError(exp, "Tick failed: {Message}", exp.Message);
            }

            await Task.Delay(_TimeoutDelayNextTick, cancellationToken);
        }

        Log.LogInformation("Task finished");
    }

    #endregion

    private async Task ProcessAsync(CancellationToken cancellationToken)
    {
        Log.LogInformation("Task started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!_MessageQueue.IsEmptyProcessQueue())
                {
                    var message = _MessageQueue.ProcessDequeue();
                    if (message != null)
                    {
                        Log.LogDebug("New message from queue. Type: {Type}, AdditionalInfo: {AdditionalInfo}, TelegramType: {TelegramType}, Text: {Text}",
                                     message.Type.AsString(),
                                     message.AdditionalInfo,
                                     message.Update?.Type,
                                     message.Update?.Message?.Text);
                        message.TryCount++;
                        if (message.IncomingMessageChatId > 0)
                        {
                            _MessageQueue.OutgoingEnqueue(new OutgoingMessage(message.IncomingMessageChatId,
                                                                              OutgoingMessageType.Typing,
                                                                              SourceProcessors.None,
                                                                              Emoji.None));
                        }
                        else
                        {
                            _MessageQueue.OutgoingEnqueue(new OutgoingMessage(OutgoingMessageType.Typing,
                                                                              UsersPermissions.Read,
                                                                              SourceProcessors.None,
                                                                              Emoji.None));
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
                    }
                    else
                    {
                        Log.LogWarning("Obtaining message is NULL!");
                    }
                }
            }
            catch (TaskCanceledException exp)
            {
                Log.LogWarning("{Message}", exp.Message);
            }
            catch (Exception exp)
            {
                Log.LogError(exp, "{Message}", exp.Message);
            }

            await Task.Delay(_TimeoutDelayNextProcess, cancellationToken);
        }

        Log.LogInformation("Task finished");
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

        var keyboard = new List<TelegramButton>(_MenuRoot.Value.Count);
        keyboard.AddRange(
                          _MenuRoot.Value.Select(
                                                 row => new TelegramButton(row.Icon.MessageCombine(row.Name))));

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

        if (_ProcessTask.Status == TaskStatus.Running)
        {
            _ProcessTask.Wait();
        }

        if (_ProcessTask.Status == TaskStatus.Running)
        {
            _TickTask.Wait();
        }

        base.Dispose();
    }
}