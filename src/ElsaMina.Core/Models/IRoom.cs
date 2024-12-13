using System.Globalization;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Models;

public interface IRoom
{
    string RoomId { get; }
    string Name { get; }
    IDictionary<string, IUser> Users { get; }
    CultureInfo Culture { get; set; }
    IGame Game { get; set; }
    RoomParameters Parameters { get; }
    IEnumerable<Tuple<string, string>> LastMessages { get; }

    void AddUser(string username);
    void RemoveUser(string username);
    void RenameUser(string oldName, string newName);
    DateTime GetUserJoinDate(string username);
    void UpdateMessageQueue(string user, string message);
}