using System.Collections.Concurrent;
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

    public void OutgoingEnqueue(OutgoingMessage message)
    {
        CheckOutgoingQueue();

        Log.LogDebug("Outgoing message: {Message}", message.ToString());
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

    public void OutgoingEnqueue(List<OutgoingMessage> messagesList)
    {
        CheckOutgoingQueue();

        foreach (var message in messagesList)
        {
            Log.LogDebug("Outgoing message: {Message}", message.ToString());
            _OutgoingQueue.Enqueue(message);
        }
    }

    public OutgoingMessage? OutgoingDequeue()
    {
        if (IsEmptyOutgoingQueue() || !_OutgoingQueue.TryDequeue(out var result))
        {
            return null;
        }

        return result;
    }

    /// <inheritdoc />
    public bool IsEmptyOutgoingQueue()
    {
        return _OutgoingQueue.IsEmpty;
    }

    #endregion

    #region ProcessEnqueue

    public void ProcessEnqueue(MessageProcess message)
    {
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

    public MessageProcess? ProcessDequeue()
    {
        if (IsEmptyProcessQueue() || !_ProcessQueue.TryDequeue(out var result))
        {
            return null;
        }

        Log.LogDebug("Process queue, obtain message: {Message}", result?.ToString());
        return result;
    }

    /// <inheritdoc />
    public bool IsEmptyProcessQueue()
    {
        return _ProcessQueue.IsEmpty;
    }

    #endregion
}