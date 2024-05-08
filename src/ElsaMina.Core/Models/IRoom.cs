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
    RoomParameters Parameters { get; set; }

    void AddUser(string user);
    void RemoveUser(string part);
    void RenameUser(string oldName, string newName);
    void EndGame();
}