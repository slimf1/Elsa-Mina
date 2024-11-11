using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGamePanelViewModel : LocalizableViewModel
{
    public ConnectFourGame ConnectFourGame { get; init; }
    public string BotName { get; set; }
    public string Trigger { get; set; }
    public string RoomId { get; set; }
}