using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace NiotTelegramBot.ModelzAndUtils.Interfaces;

public interface IPluginDataSource : IDisposable
{
    public string Name { get; }
    public bool Enabled { get; set; }
    public Task<bool> Healthcheck();
    public Task<List<T>> Many<T, TKey, TValue>(string commandText, List<KeyValuePair<TKey, TValue>> parameterList) where T : class;
    public Task<T> Single<T, TKey, TValue>(string commandText, List<KeyValuePair<TKey, TValue>> parameterList) where T : class;
}