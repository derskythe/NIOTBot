// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace ModelzAndUtils.Settings;

/// <inheritdoc />
public class PluginOutgoingInputSettings : PluginAbstractSettings
{
    public string InputDir { get; set; } = string.Empty;
    public string OutputDir { get; set; } = string.Empty;
    public string Options { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{base.ToString()}, InputDir: {InputDir}, OutputDir: {OutputDir}, Options: {Options}";
    }
}

public class PluginInputArraySettings
{
    public const string NAME = "PluginsInput";

    public List<PluginOutgoingInputSettings> List { get; set; } = new(0);

    /// <inheritdoc />
    public override string ToString()
    {
        return $"List: {List.ShowCount()}";
    }
}