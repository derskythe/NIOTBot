#nullable enable

using ModelzAndUtils.Models;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ModelzAndUtils.Interfaces;

public interface IMessageQueueService
{
    void OutgoingEnqueue(OutgoingMessage message);

    void OutgoingEnqueue(List<OutgoingMessage> messagesList);

    OutgoingMessage? OutgoingDequeue();

    void ProcessEnqueue(MessageProcess message);

    MessageProcess? ProcessDequeue();
}