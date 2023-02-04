// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace NiotTelegramBot.ModelzAndUtils.Settings;

public class ProcessorSettings
{
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public string Options { get; set; } = string.Empty;
    public int Order { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Name: {Name}, Enabled: {Enabled}, Options: {Options}, Order: {Order}";
    }
}

public class PluginProcessorArraySettings
{
    public const string NAME = "PluginsProcessor";

    public IReadOnlyList<ProcessorSettings> List { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"List: {List.ShowCount()}";
    }
}