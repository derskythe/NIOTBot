// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace NiotTelegramBot.ModelzAndUtils.Settings;

public class OutgoingInputSettings
{
    public const string NAME = "PluginsOutgoingInput";
    
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public string InputDir { get; set; } = string.Empty;
    public string OutputDir { get; set; } = string.Empty;
    public string Options { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({Enabled}),, InputDir: {InputDir}, OutputDir: {OutputDir}, Options: {Options}";
    }
}