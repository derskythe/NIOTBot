// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace ModelzAndUtils.Settings;

public abstract class PluginAbstractSettings
{
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Name: {Name}, Enabled: {Enabled}";
    }
}