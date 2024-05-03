namespace ElsaMina.Core.Models;

public class RoomBotConfigurationParameter : IRoomBotConfigurationParameter
{
    public string Identifier { get; set; }
    public string NameKey { get; set; }
    public string DescriptionKey { get; set; }
    public RoomBotConfigurationType Type { get; set; }
}