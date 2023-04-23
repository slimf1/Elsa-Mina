namespace ElsaMina.Core.Models;

public interface IRoom
{
    string RoomId { get; }
    string Name { get; }
    IDictionary<string, IUser> Users { get; }
    string Locale { get; }
    
    void AddUser(string user);
    void RemoveUser(string part);
    void RenameUser(string oldName, string newName);
}