using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.RoomDashboard;

public class RoomDashboardViewModel : LocalizableViewModel
{
    public string BotName { get; set; }
    public string Trigger { get; set; }
    public IEnumerable<RoomParameterLineModel> RoomParameterLines { get; set; }
    public string RoomName { get; set; }
    public string RoomId { get; set; }
    public string Command { get; set; }
}