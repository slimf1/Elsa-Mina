using ElsaMina.Core.Models;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.RoomDashboard;

public class RoomParameterLineModel : LocalizableViewModel
{
    public IRoomBotConfigurationParameter RoomParameter { get; init; }
    public string CurrentValue { get; init; }
}