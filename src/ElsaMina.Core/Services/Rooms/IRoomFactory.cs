namespace ElsaMina.Core.Services.Rooms;

public interface IRoomFactory
{
    Task<IRoom> CreateRoomAsync(string roomId, string[] lines, CancellationToken cancellationToken = default);
}
