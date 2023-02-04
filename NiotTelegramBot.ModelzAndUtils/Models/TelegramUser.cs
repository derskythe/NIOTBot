// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using NiotTelegramBot.ModelzAndUtils.Enums;

namespace NiotTelegramBot.ModelzAndUtils.Models;

public class TelegramUser
{
    public string Username { get; set; } = string.Empty;
    public long ChatId { get; set; }
    public UsersPermissions Permission { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Username: {Username}, ChatId: {ChatId}, Permission: {Permission.AsString()}";
    }
}