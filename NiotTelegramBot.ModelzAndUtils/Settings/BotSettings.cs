// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
namespace NiotTelegramBot.ModelzAndUtils.Settings;

public class BotSettings
{
    public const string NAME = "BotSettings";
    public string Token { get; set; } = string.Empty;
    public string AllowedUsernames { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var token = !string.IsNullOrEmpty(Token) && Token.Contains(':') ? Constants.VALID : Constants.EMPTY;
        return $"Token: {token}, AllowedUsernames: {AllowedUsernames}";
    }
}