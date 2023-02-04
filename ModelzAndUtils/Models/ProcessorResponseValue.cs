// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable UnusedMember.Global
#nullable enable
namespace ModelzAndUtils.Models;

public struct ProcessorResponseValue
{
    public bool IsException { get; set; }
    public string Message { get; set; }
    public List<OutgoingMessage>? Responses { get; set; }

    public ProcessorResponseValue()
    {
        IsException = false;
        Message = string.Empty;
        Responses = null;
    }

    /// <inheritdoc />
    public ProcessorResponseValue(string message)
        : this()
    {
        IsException = true;
        Message = message;
    }

    public ProcessorResponseValue(string message, List<OutgoingMessage> responses)
    {
        IsException = true;
        Message = message;
        Responses = responses;
    }

    /// <inheritdoc />
    public ProcessorResponseValue(List<OutgoingMessage> responses)
        : this()
    {
        Message = string.Empty;
        Responses = responses;
    }

    public static ProcessorResponseValue SingleOutgoingMessage(OutgoingMessage message)
    {
        return new ProcessorResponseValue(new List<OutgoingMessage>()
                                          {
                                              message
                                          });
    }

    // ReSharper disable once UnusedMember.Global
    public static ProcessorResponseValue SingleOutgoingMessage(OutgoingMessage message, string exceptionMessage)
    {
        return new ProcessorResponseValue(exceptionMessage,
                                          new List<OutgoingMessage>()
                                          {
                                              message
                                          });
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var response = Responses == null ? "empty" : Responses.Count.ToString();
        return $"IsException: {IsException}, Message: {Message}, Response: {response}";
    }
}