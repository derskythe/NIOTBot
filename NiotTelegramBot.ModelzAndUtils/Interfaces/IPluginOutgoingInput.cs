using Microsoft.Extensions.Hosting;
using NiotTelegramBot.ModelzAndUtils.Settings;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace NiotTelegramBot.ModelzAndUtils.Interfaces;

public interface IPluginOutgoingInput : IHostedService
{
    public string Name { get; }
    public bool Enabled { get; set; }
    public void Init(PluginOutgoingInputSettings outgoingInputSettings);
    public bool Healthcheck();
}