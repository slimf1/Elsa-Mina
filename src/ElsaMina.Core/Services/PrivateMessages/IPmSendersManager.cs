using ElsaMina.Core.Models;

namespace ElsaMina.Core.Services.PrivateMessages;

public interface IPmSendersManager
{
    bool HasUser(string userId);
    IUser GetUser(string username);
}