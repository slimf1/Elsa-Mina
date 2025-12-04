namespace ElsaMina.Core.Services.Rooms.Parameters;
using DbRoom = DataAccess.Models.Room;

public interface IRoomParameterStore
{
    string GetValue(Parameter parameter);
    Task<string> GetValueAsync(Parameter parameter, CancellationToken cancellationToken = default);
    bool SetValue(Parameter parameter, string value);
    Task<bool> SetValueAsync(Parameter parameter, string value, CancellationToken cancellationToken = default);
    IRoom Room { get; set; }
    void InitializeFromRoomEntity(DbRoom roomEntity);
}