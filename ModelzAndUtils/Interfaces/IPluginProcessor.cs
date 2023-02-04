#nullable enable
using System.Threading.Tasks;
using ModelzAndUtils.Enums;
using ModelzAndUtils.Models;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ModelzAndUtils.Interfaces;

public interface IPluginProcessor
{
    Emoji Icon { get; set; }
    public string NameForUser { get; set; }
    public TelegramMenu[] Menu { get; set; }
    public string Name { get; }
    public SourceProcessors SourceSourceProcessor { get; }
    public bool Enabled { get; set; }
    public int Order { get; set; }
    public bool Healthcheck();
    public Task<ProcessorResponseValue> Tick();
    public Task<ProcessorResponseValue> Process(MessageProcess messageProcess);
}