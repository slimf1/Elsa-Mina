using ElsaMina.Core.Models;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoomsManager
{
    IRoom GetRoom(string roomId);
    bool HasRoom(string roomId);
    Task InitializeRoom(string roomId, string roomTitle, IEnumerable<string> userIds);
    void RemoveRoom(string roomId);
    void AddUserToRoom(string roomId, string userId);
    void RemoveUserFromRoom(string roomId, string userId);
    void RenameUserInRoom(string roomId, string newName, string lastName);
}