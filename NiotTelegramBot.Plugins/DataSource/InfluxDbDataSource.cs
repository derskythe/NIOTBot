using InfluxDB.Client;

namespace NiotTelegramBot.Plugins.DataSource;

public class InfluxDbDataSource : IPluginDataSource
{
    // ReSharper disable once InconsistentNaming
    private readonly ILogger<InfluxDbDataSource> Log;
    private readonly DataSourceSettings _Settings;
    private readonly string _Uri;
    private readonly string _Token;
    private readonly InfluxDBClient _Client;
    private readonly string _Bucket;
    private bool _IsDisposed;
    private readonly CancellationToken _CancelationToken;
    private readonly string _Org;

    public InfluxDbDataSource(ILoggerFactory loggerFactory, DataSourceSettings settings, CancellationToken cancellationToken)
    {
        Log = loggerFactory.CreateLogger<InfluxDbDataSource>();
        _CancelationToken = cancellationToken;
        _Settings = settings;
        _Uri = new UriBuilder(settings.Proto.IsEqual("https") ? "https" : "http",
                              settings.Hostname,
                              settings.Port,
                              "/",
                              $"timeout={_Settings.Timeout * 1000}").Uri.ToString();
        _Token = settings.Password;
        _Bucket = settings.DatabaseName;
        _Org = settings.Username;
        Name = settings.Name;
        Enabled = settings.Enabled;

        Log.LogInformation("{Name} URI: {Uri}, Bucket: {Bucket}, Organization: {Organization}, Enabled: {Enabled}",
                           Name,
                           _Uri,
                           _Bucket,
                           _Org,
                           Enabled);
        _Client = new InfluxDBClient(_Uri, _Token);
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public async Task<bool> Healthcheck()
    {
        return Enabled && await _Client.PingAsync();
    }

    /// <inheritdoc />
    public async Task<List<T>> Many<T, TKey, TValue>(string commandText, List<KeyValuePair<TKey, TValue>> parameterList)
        where T : class
    {
        if (!await Healthcheck())
        {
            return new List<T>(0);
        }

        var flux = $"from(bucket:\"{_Bucket}\") |> {commandText}"; // range(start: 0)
        Log.LogDebug("Request: {Request}", flux);
        var queryApi = _Client.GetQueryApi();

        return await queryApi.QueryAsync<T>(flux, _Org, _CancelationToken);
    }

    /// <inheritdoc />
    public async Task<T?> Single<T, TKey, TValue>(string commandText, List<KeyValuePair<TKey, TValue>> parameterList)
        where T : class
    {
        if (!await Healthcheck())
        {
            return null;
        }

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_IsDisposed)
        {
            _IsDisposed = true;
            _Client.Dispose();
        }
    }
}