using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.RoomDashboard;

public class RoomParameterLineModel : LocalizableViewModel
{
    public IParameterDefiniton RoomParameterDefiniton { get; init; }
    public string CurrentValue { get; init; }
}