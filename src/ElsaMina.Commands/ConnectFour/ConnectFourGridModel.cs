using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGridModel : LocalizableViewModel
{
    public IConnectFourGame CurrentGame { get; set; }
    public string BotName { get; set; }
    public string Trigger { get; set; }
    public string RoomId { get; set; }
}