using System.Globalization;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoom
{
    string RoomId { get; }
    string Name { get; }
    IReadOnlyDictionary<string, IUser> Users { get; }
    CultureInfo Culture { get; set; }
    IGame Game { get; set; }
    RoomInfo Info { get; }
    IEnumerable<Tuple<string, string>> LastMessages { get; }

    void AddUser(string username);
    void RemoveUser(string username);
    void RenameUser(string oldName, string newName);
    DateTime GetUserJoinDate(string username);
    void UpdateMessageQueue(string user, string message);
}