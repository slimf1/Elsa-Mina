using ElsaMina.Core.Services.Rooms.Parameters;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoomsManager
{
    IReadOnlyDictionary<string, IParameter> RoomParameters { get; }
    void Initialize();
    IRoom GetRoom(string roomId);
    bool HasRoom(string roomId);
    Task InitializeRoomAsync(string roomId, IEnumerable<string> lines, CancellationToken cancellationToken = default);
    void RemoveRoom(string roomId);
    void AddUserToRoom(string roomId, string username);
    void RemoveUserFromRoom(string roomId, string username);
    void RenameUserInRoom(string roomId, string formerName, string newName);
    string GetRoomParameter(string roomId, string parameterId);
    Task<bool> SetRoomParameter(string roomId, string parameterId, string value);
    Task ProcessPendingPlayTimeUpdates();
    void Clear();
}