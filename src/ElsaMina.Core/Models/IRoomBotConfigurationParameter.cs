namespace ElsaMina.Core.Models;

public interface IRoomBotConfigurationParameter
{
    string Identifier { get; }
    string NameKey { get; }
    string DescriptionKey { get; }
    RoomBotConfigurationType Type { get; }
}