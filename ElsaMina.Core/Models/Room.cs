using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Models;

public class Room : IRoom
{
    public string RoomId { get; }
    public string Name { get; }
    public IDictionary<string, IUser> Users { get; } = new Dictionary<string, IUser>();
    public string Locale { get; set; }
    public IGame Game { get; set; }

    public Room(string roomTitle, string roomId, string locale)
    {
        RoomId = roomId ?? roomTitle.ToLowerAlphaNum();
        Name = roomTitle;
        Locale = locale;
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
               $"{nameof(Locale)}: {Locale}]";
    }
}