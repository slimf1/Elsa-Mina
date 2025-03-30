using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGamePanelViewModel : LocalizableViewModel
{
    public IConnectFourGame ConnectFourGame { get; init; }
    public string BotName { get; init; }
    public string Trigger { get; init; }
    public string RoomId { get; init; }
}