namespace ElsaMina.Core.Models;

public interface IRoom
{
    void AddUser(string user);
    void RemoveUser(string part);
    void RenameUser(string oldName, string newName);
}