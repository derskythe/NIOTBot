// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace NiotTelegramBot.ModelzAndUtils.Settings;

[Serializable]
public class DataSourceSettings
{
    public const string NAME = "PluginsDataSource";

    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Proto { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Timeout { get; set; } = -1;

    /// <inheritdoc />
    public override string ToString()
    {
        var password = !string.IsNullOrEmpty(Password) ? Constants.VALID : Constants.EMPTY;
        return
            $"{Name} ({Enabled}), Hostname: {Hostname}, Port: {Port}, Proto: {Proto}, DatabaseName: {DatabaseName}, Username: {Username}, " +
            $"Password: {password}, Timeout: {Timeout}";
    }
}

public class PluginDataSourceArraySettings
{
    public IReadOnlyList<DataSourceSettings> List { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"List: {List.ShowCount()}";
    }
}