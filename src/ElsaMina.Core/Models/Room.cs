using System.Globalization;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Models;

public class Room : IRoom
{
    public string RoomId { get; }
    public string Name { get; }
    public IDictionary<string, IUser> Users { get; } = new Dictionary<string, IUser>();
    public CultureInfo Culture { get; set; }
    public IGame Game { get; set; }
    public RoomParameters Parameters { get; set; }

    public Room(string roomTitle, string roomId, CultureInfo culture)
    {
        RoomId = roomId ?? roomTitle.ToLowerAlphaNum();
        Name = roomTitle;
        Culture = culture;
    }

    public void AddUser(string username)
    {
        var user = new User(username[1..], username[0]);
        Users[user.UserId] = user;
    }
    
    public void RemoveUser(string username)
    {
        Users.Remove(username.ToLowerAlphaNum());
    }

    public void RenameUser(string oldName, string newName)
    {
        RemoveUser(oldName);
        AddUser(newName);
    }

    public void EndGame()
    {
        Game?.Cancel();
        Game = null;
    }

    public override string ToString()
    {
        return $"{nameof(Room)}[{nameof(RoomId)}: {RoomId}, " +
               $"{nameof(Name)}: {Name}, " +
               $"{nameof(Users)}: {string.Join(", ", Users)}, " +
               $"{nameof(Culture)}: {Culture}]";
    }
}