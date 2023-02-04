// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace NiotTelegramBot.ModelzAndUtils.Models;

public class TelegramButton
{
    public string Text { get; }
    public string CallbackData { get; }
    public bool InlineKeyboard { get; }
    public bool IsPhoneNumberRequest { get; }
    public bool IsLocationRequest { get; }

    public TelegramButton(string text)
    {
        Text = text;
        InlineKeyboard = false;
        IsPhoneNumberRequest = false;
        IsLocationRequest = false;
    }

    public TelegramButton(string text, bool isPhoneNumberRequest, bool isLocationRequest)
    {
        Text = text;
        InlineKeyboard = false;
        IsPhoneNumberRequest = isPhoneNumberRequest;
        IsLocationRequest = isLocationRequest;
    }

    public TelegramButton(string text, string callbackData)
    {
        Text = text;
        CallbackData = callbackData;
        InlineKeyboard = true;
        IsPhoneNumberRequest = false;
        IsLocationRequest = false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var phoneNumberRequest = IsPhoneNumberRequest ? " PhoneNumberRequest " : string.Empty;
        var locationRequest = IsLocationRequest ? " LocationRequest " : string.Empty;
        return $"Text: {Text}, {phoneNumberRequest}{locationRequest} CallbackData: {CallbackData}, InlineKeyboard: {InlineKeyboard}";
    }
}