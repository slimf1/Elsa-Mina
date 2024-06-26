﻿using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGameTableModel : LocalizableViewModel
{
    public ConnectFour CurrentGame { get; set; }
    public string Trigger { get; set; }
    public string BotName { get; set; }
}