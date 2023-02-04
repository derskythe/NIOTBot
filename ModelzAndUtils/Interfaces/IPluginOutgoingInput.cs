using Microsoft.Extensions.Hosting;
using ModelzAndUtils.Settings;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ModelzAndUtils.Interfaces;

public interface IPluginOutgoingInput : IHostedService
{
    public string Name { get; }
    public bool Enabled { get; set; }
    public void Init(PluginOutgoingInputSettings outgoingInputSettings);
    public bool Healthcheck();
}