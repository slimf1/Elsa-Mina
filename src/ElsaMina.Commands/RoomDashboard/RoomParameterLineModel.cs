using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.RoomDashboard;

public class RoomParameterLineModel : LocalizableViewModel
{
    public IRoomBotConfigurationParameter RoomParameter { get; init; }
    public string CurrentValue { get; init; }
}