using ElsaMina.Core.Models;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoomsManager
{
    IReadOnlyDictionary<string, IRoomBotConfigurationParameter> RoomBotConfigurationParameters { get; }
    IRoom GetRoom(string roomId);
    bool HasRoom(string roomId);
    Task InitializeRoomAsync(string roomId, IEnumerable<string> lines, CancellationToken cancellationToken = default);
    void RemoveRoom(string roomId);
    void AddUserToRoom(string roomId, string username);
    void RemoveUserFromRoom(string roomId, string username);
    void RenameUserInRoom(string roomId, string formerName, string newName);
    string GetRoomBotConfigurationParameterValue(string roomId, string roomBotParameterId);
    Task<bool> SetRoomBotConfigurationParameterValue(string roomId, string roomBotParameterId, string value);
    void Clear();
}