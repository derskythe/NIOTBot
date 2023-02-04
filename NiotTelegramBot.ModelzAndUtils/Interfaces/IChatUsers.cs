using System.Threading.Tasks;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Models;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace NiotTelegramBot.ModelzAndUtils.Interfaces;

public interface IChatUsers
{
    public bool IsAuthorized(string username);
    public Task AddUser(string username, long chatId);
    public Task RemoveUser(string username);
    public TelegramUser GetByUsername(string username);
    public TelegramUser GetByChatId(long chatId);
    public bool HasPermission(string username, UsersPermissions permissions);
    public List<TelegramUser> ListUsersByPermission(UsersPermissions all);
    public Task UpdateChatId(string username, long chatId);
}