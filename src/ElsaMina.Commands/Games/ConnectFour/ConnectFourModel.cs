using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Games.ConnectFour;

public class ConnectFourModel : LocalizableViewModel
{
    public IConnectFourGame CurrentGame { get; init; }
    public string Trigger { get; init; }
    public string BotName { get; init; }
    public string RoomId { get; init; }
}