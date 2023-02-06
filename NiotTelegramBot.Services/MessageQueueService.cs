using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using EnumsNET;
using Microsoft.Extensions.Logging;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;

namespace NiotTelegramBot.Services;

/// <inheritdoc />
public class MessageQueueService : IMessageQueueService
{
    #region Constructor and fields

    // ReSharper disable once InconsistentNaming
    private readonly ILogger<MessageQueueService> Log;
    private readonly ConcurrentQueue<OutgoingMessage> _OutgoingQueue;
    private readonly ConcurrentQueue<MessageProcess> _ProcessQueue;

    public MessageQueueService(ILogger<MessageQueueService> log)
    {
        Log = log;
        _OutgoingQueue = new ConcurrentQueue<OutgoingMessage>();
        _ProcessQueue = new ConcurrentQueue<MessageProcess>();
    }

    #endregion

    #region OutgoingEnqueue

    public void OutgoingEnqueue(
        OutgoingMessage message,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        CheckOutgoingQueue();
#if DEBUG
        if (string.IsNullOrEmpty(callerMemberName))
        {
            callerMemberName = new StackTrace().GetFrame(1)?.GetMethod()?.Name ?? string.Empty;
        }
#endif

        Log.LogDebug("Outgoing message: {Message}, Method: {Method}, Path: {Path}, Line: {Line}",
                     message.ToString(),
                     callerMemberName,
                     callerFilePath,
                     callerLineNumber);
        _OutgoingQueue.Enqueue(message);
    }

    private void CheckOutgoingQueue()
    {
        var count = _OutgoingQueue.Count;
        if (count > 0 && count % 5 == 0)
        {
            Log.LogWarning("Incoming message queue is expanding! Count: {Count}", count);
        }
        else
        {
            Log.LogDebug("Incoming message queue count: {Count}", count);
        }
    }

    public void OutgoingEnqueue(
        List<OutgoingMessage> messagesList,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        CheckOutgoingQueue();
#if DEBUG
        if (string.IsNullOrEmpty(callerMemberName))
        {
            callerMemberName = new StackTrace().GetFrame(1)?.GetMethod()?.Name ?? string.Empty;
        }
#endif
        foreach (var message in messagesList)
        {
            Log.LogDebug("MessageType: {Type}, Text: {TexT}, Method: {Method}, Path: {Path}, Line: {Line}",
                         message.Type.AsString(),
                         message.ToString(),
                         callerMemberName,
                         callerFilePath,
                         callerLineNumber);
            _OutgoingQueue.Enqueue(message);
        }
    }

    public OutgoingMessage? OutgoingDequeue(
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (IsEmptyOutgoingQueue() || !_OutgoingQueue.TryDequeue(out var result))
        {
            return null;
        }
#if DEBUG
        if (string.IsNullOrEmpty(callerMemberName))
        {
            callerMemberName = new StackTrace().GetFrame(1)?.GetMethod()?.Name ?? string.Empty;
        }
#endif
        Log.LogDebug("MessageType: {Type}, Text: {TexT}, Method: {Method}, Path: {Path}, Line: {Line}",
                     result.Type.AsString(),
                     result.Text,
                     callerMemberName,
                     callerFilePath,
                     callerLineNumber);

        return result;
    }

    /// <inheritdoc />
    public bool IsEmptyOutgoingQueue()
    {
        return _OutgoingQueue.IsEmpty;
    }

    #endregion

    #region ProcessEnqueue

    public void ProcessEnqueue(
        MessageProcess message,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
#if DEBUG
        if (string.IsNullOrEmpty(callerMemberName))
        {
            callerMemberName = new StackTrace().GetFrame(1)?.GetMethod()?.Name ?? string.Empty;
        }
#endif
        if (!string.IsNullOrEmpty(callerMemberName))
        {
            Log.LogDebug("MessageType: {Type}, Text: {Text}, Method: {Method}, Path: {Path}, Line: {Line}",
                         message.Type.AsString(),
                         message.AdditionalInfo,
                         callerMemberName,
                         callerFilePath,
                         callerLineNumber);
        }

        CheckProcessQueue();
        _ProcessQueue.Enqueue(message);

#if DEBUG
        CheckProcessQueue();
#endif
    }

    private void CheckProcessQueue()
    {
        var count = _ProcessQueue.Count;
        if (count > 0 && count % 5 == 0)
        {
            Log.LogWarning("Process queue is expanding! Count: {Count}", count);
        }
        else
        {
            Log.LogDebug("Process queue count: {Count}", count);
        }
    }

    public MessageProcess? ProcessDequeue(
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (IsEmptyProcessQueue() || !_ProcessQueue.TryDequeue(out var result))
        {
            return null;
        }
#if DEBUG
        if (string.IsNullOrEmpty(callerMemberName))
        {
            callerMemberName = new StackTrace().GetFrame(1)?.GetMethod()?.Name ?? string.Empty;
        }
#endif
        Log.LogDebug("Process queue, obtain message: {Message}, Method: {Method}, Path: {Path}, Line: {Line}",
                     result.ToString(),
                     callerMemberName,
                     callerFilePath,
                     callerLineNumber);
        return result;
    }

    /// <inheritdoc />
    public bool IsEmptyProcessQueue()
    {
        return _ProcessQueue.IsEmpty;
    }

    #endregion
}