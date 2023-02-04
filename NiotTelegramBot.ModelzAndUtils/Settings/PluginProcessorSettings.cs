// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace NiotTelegramBot.ModelzAndUtils.Settings;

public class PluginProcessorSettings : PluginAbstractSettings
{
    public string Options { get; set; } = string.Empty;
    public int Order { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{base.ToString()}, Options: {Options}";
    }
}

public class PluginProcessorArraySettings
{
    public const string NAME = "PluginsProcessor";

    public List<PluginProcessorSettings> List { get; set; } = new(0);

    /// <inheritdoc />
    public override string ToString()
    {
        return $"List: {List.ShowCount()}";
    }
}