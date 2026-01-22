using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGameTableModel : LocalizableViewModel
{
    public IConnectFourGame CurrentGame { get; init; }
    public string Trigger { get; init; }
    public string BotName { get; init; }
    public string RoomId { get; init; }
}