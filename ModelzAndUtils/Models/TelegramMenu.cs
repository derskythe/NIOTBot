using ModelzAndUtils.Enums;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace ModelzAndUtils.Models;

public class TelegramMenu
{
    public Emoji Icon { get; }
    public string Name { get; }
    public string AdditionalData { get; }
    public SourceProcessors SourceProcessor { get; }
    public int Order { get; }

    public TelegramMenu(Emoji icon, string name, int order, SourceProcessors sourceProcessor)
    {
        Icon = icon;
        Name = name;
        AdditionalData = string.Empty;
        Order = order;
        SourceProcessor = sourceProcessor;
    }

    public TelegramMenu(Emoji icon, string name, string additionalData, int order, SourceProcessors sourceProcessor)
    {
        Icon = icon;
        Name = name;
        AdditionalData = additionalData;
        Order = order;
        SourceProcessor = sourceProcessor;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Order}. {Icon} {Name}, Processor: {SourceProcessor.AsString()}, AdditionalData: {AdditionalData}";
    }
}