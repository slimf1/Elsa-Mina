using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Services.Rooms.Parameters;

public interface IRoomParameterStore
{
    Task<string> GetValueAsync(Parameter parameter, CancellationToken cancellationToken = default);
    Task<bool> SetValueAsync(Parameter parameter, string value, CancellationToken cancellationToken = default);
    IRoom Room { get; set; }
    void InitializeFromRoomEntity(SavedRoom savedRoomEntity);
}