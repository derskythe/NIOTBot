using System.Threading.Tasks;
using ModelzAndUtils.Enums;
using ModelzAndUtils.Models;
using ModelzAndUtils.Settings;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ModelzAndUtils.Interfaces;

public interface IChatUsers
{
    public bool IsAuthorized(string username);
    public Task AddUser(string username, long chatId);
    public Task RemoveUser(string username);
    public long GetChatId(string username);
    public string GetUsername(long chatId);
    public bool HasPermission(string username, UsersPermissions permissions);
    public List<TelegramUser> ListUsersByPermission(UsersPermissions all);
}