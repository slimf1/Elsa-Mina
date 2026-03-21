using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Arcade.Events;

public class EventRoleMappingViewModel : LocalizableViewModel
{
    public required string BotName { get; init; }
    public required string Trigger { get; init; }
    public required string RoomId { get; init; }
    public required IReadOnlyList<EventRoleMapping> Mappings { get; init; }
}
