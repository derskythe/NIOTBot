using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using ModelzAndUtils.Interfaces;
using ModelzAndUtils.Models;

namespace NiotTelegramBot.Services;

/// <inheritdoc />
public class MessageQueueService : IMessageQueueService
{
    #region Constructor and fields

    // ReSharper disable once InconsistentNaming
    private readonly ILogger<MessageQueueService> Log;
    private readonly ConcurrentQueue<OutgoingMessage> _IncomingQueue;
    private readonly ConcurrentQueue<MessageProcess> _ProcessQueue;

    public MessageQueueService(ILogger<MessageQueueService> log)
    {
        Log = log;
        _IncomingQueue = new ConcurrentQueue<OutgoingMessage>();
        _ProcessQueue = new ConcurrentQueue<MessageProcess>();
    }

    #endregion
        
    #region OutgoingEnqueue

    public void OutgoingEnqueue(OutgoingMessage message)
    {
        _IncomingQueue.Enqueue(message);
    }

    public void OutgoingEnqueue(List<OutgoingMessage> messagesList)
    {
        var count = _IncomingQueue.Count;
        if (count % 5 == 0)
        {
            Log.LogWarning("Incoming message queue is expanding! Count: {Count}", count);
        }
        foreach (var message in messagesList)
        {
            _IncomingQueue.Enqueue(message);
        }
    }

    public OutgoingMessage? OutgoingDequeue()
    {
        if (_IncomingQueue.IsEmpty || _IncomingQueue.TryDequeue(out var result))
        {
            return null;
        }

        return result;
    }

    #endregion

    #region ProcessEnqueue

    public void ProcessEnqueue(MessageProcess message)
    {
        var count = _ProcessQueue.Count;
        if (count % 5 == 0)
        {
            Log.LogWarning("Process queue is expanding! Count: {Count}", count);
        }
        _ProcessQueue.Enqueue(message);
    }

    public MessageProcess? ProcessDequeue()
    {
        if (_ProcessQueue.IsEmpty || _ProcessQueue.TryDequeue(out var result))
        {
            return null;
        }

        return result;
    }

    #endregion
}