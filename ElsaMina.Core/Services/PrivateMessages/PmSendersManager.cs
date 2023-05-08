using ElsaMina.Core.Models;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.PrivateMessages;

public class PmSendersManager : IPmSendersManager
{
    private readonly Dictionary<string, IUser> _users = new();

    public bool HasUser(string userId)
    {
        return _users.ContainsKey(userId);
    }

    public IUser GetUser(string userName)
    {
        var userId = userName.ToLowerAlphaNum();
        if (HasUser(userId))
        {
            return _users[userId];
        }

        var user = new User(userName[1..], userName[0]);
        _users[userId] = user;
        return user;
    }
}