using ElsaMina.Core.Models;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoomsManager
{
    IReadOnlyDictionary<string, IRoomBotConfigurationParameter> RoomBotConfigurationParameters { get; }
    IRoom GetRoom(string roomId);
    bool HasRoom(string roomId);
    Task InitializeRoom(string roomId, string roomTitle, IEnumerable<string> userIds);
    void RemoveRoom(string roomId);
    void AddUserToRoom(string roomId, string userId);
    void RemoveUserFromRoom(string roomId, string userId);
    void RenameUserInRoom(string roomId, string formerName, string newName);
    string GetRoomBotConfigurationParameterValue(string roomId, string roomBotParameterId);
    Task<bool> SetRoomBotConfigurationParameterValue(string roomId, string roomBotParameterId, string value);
}