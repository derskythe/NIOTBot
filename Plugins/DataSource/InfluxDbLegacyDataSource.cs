using InfluxDB.Client.Flux;

namespace Plugins.DataSource;

public class InfluxDbLegacyDataSource : IPluginDataSource
{
    // ReSharper disable once InconsistentNaming
    private readonly ILogger<InfluxDbLegacyDataSource> Log;
    private readonly string _Uri;
    private readonly FluxClient _Client;
    private readonly string _Bucket;
    private bool _IsDisposed;
    private readonly CancellationToken _CancelationToken;
    private readonly string _Username;
    private readonly string _Password;

    public InfluxDbLegacyDataSource(
        ILoggerFactory logggerFactory,
        PluginDataSourceSettings settings,
        CancellationToken cancellationToken)
    {
        Log = logggerFactory.CreateLogger<InfluxDbLegacyDataSource>();
        _CancelationToken = cancellationToken;
        _Uri = new UriBuilder(settings.Proto.IsEqual("https") ? "https" : "http",
                              settings.Hostname,
                              settings.Port).Uri.ToString();
        _Bucket = settings.DatabaseName;
        _Username = settings.Username;
        _Password = settings.Password;
        Name = settings.Name;
        Enabled = settings.Enabled;

        Log.LogInformation("{Name} URI: {Uri}, Bucket: {Bucket}, Username: {Username}, Enabled: {Enabled}",
                           Name,
                           _Uri,
                           _Bucket,
                           _Username,
                           Enabled);
        // client creation
        var options = new FluxConnectionOptions(_Uri,
                                                TimeSpan.FromSeconds(settings.Timeout),
                                                _Username,
                                                _Password.ToCharArray());
        _Client = new FluxClient(options);
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public async Task<bool> Healthcheck()
    {
        return Enabled && await _Client.PingAsync(_CancelationToken);
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
        return await _Client.QueryAsync<T>(flux, _CancelationToken);
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
        if (_IsDisposed)
        {
            return;
        }
        _IsDisposed = true;
        _Client.Dispose();
    }
}