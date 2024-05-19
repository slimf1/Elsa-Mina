using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGridModel : LocalizableViewModel
{
    public ConnectFour CurrentGame { get; set; }
    public string BotName { get; set; }
    public string Trigger { get; set; }
}