using ElsaMina.Core.Services.Rooms.Parameters;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoomsManager
{
    IReadOnlyDictionary<Parameter, IParameterDefinition> ParametersDefinitions { get; }
    void Initialize();
    IRoom GetRoom(string roomId);
    bool HasRoom(string roomId);
    Task InitializeRoomAsync(string roomId, IEnumerable<string> lines, CancellationToken cancellationToken = default);
    void RemoveRoom(string roomId);
    void AddUserToRoom(string roomId, string username);
    void RemoveUserFromRoom(string roomId, string username);
    void RenameUserInRoom(string roomId, string formerName, string newName);
    Task ProcessPendingPlayTimeUpdates();
    void Clear();
    Task WaitForPlayTimeUpdatesAsync(CancellationToken cancellationToken = default);
}