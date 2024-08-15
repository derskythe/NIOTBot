#nullable enable

using NiotTelegramBot.ModelzAndUtils.Models;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace NiotTelegramBot.ModelzAndUtils.Interfaces;

public interface IMessageQueueService
{
    void OutgoingEnqueue(OutgoingMessage message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

    void OutgoingEnqueue(List<OutgoingMessage> messagesList, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

    OutgoingMessage? OutgoingDequeue(string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

    void ProcessEnqueue(MessageProcess message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

    MessageProcess? ProcessDequeue(string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

    bool IsEmptyProcessQueue();

    bool IsEmptyOutgoingQueue();
}