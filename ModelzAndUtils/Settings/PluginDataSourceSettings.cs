// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace ModelzAndUtils.Settings;

public class PluginDataSourceSettings : PluginAbstractSettings
{
    public string Hostname { get; set; }= string.Empty;
    public int Port { get; set; }
    public string Proto { get; set; }= string.Empty;
    public string DatabaseName { get; set; }= string.Empty;
    public string Username { get; set; }= string.Empty;
    public string Password { get; set; }= string.Empty;
    public int Timeout { get; set; } = -1;

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"{base.ToString()}, Hostname: {Hostname}, Port: {Port}, Proto: {Proto}, DatabaseName: {DatabaseName}, Username: {Username}, Password: {Password}, Timeout: {Timeout}";
    }
}

public class PluginDataSourceArraySettings
{
    public const string NAME = "PluginsDataSource";

    public List<PluginDataSourceSettings> List { get; set; } = new(0);

    /// <inheritdoc />
    public override string ToString()
    {
        return $"List: {List.ShowCount()}";
    }
}