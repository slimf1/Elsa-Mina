using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Arcade.Events;

public interface IEventRoleMappingService
{
    Task<IReadOnlyList<EventRoleMapping>> GetMappingsForRoomAsync(string roomId, CancellationToken cancellationToken = default);
    Task<EventRoleMapping> GetMappingAsync(string eventName, string roomId, CancellationToken cancellationToken = default);
    Task SaveMappingAsync(EventRoleMapping mapping, CancellationToken cancellationToken = default);
    Task DeleteMappingAsync(string eventName, string roomId, CancellationToken cancellationToken = default);
}
