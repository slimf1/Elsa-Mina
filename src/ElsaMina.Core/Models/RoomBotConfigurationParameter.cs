using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Models;

public class RoomBotConfigurationParameter : IRoomBotConfigurationParameter
{
    public string Identifier { get; set; }
    public string NameKey { get; set; }
    public string DescriptionKey { get; set; }
    public RoomBotConfigurationType Type { get; set; }
    public string DefaultValue { get; set; }
    public IEnumerable<EnumerationValue> PossibleValues { get; set; } // Only used for the enumeration type
    public Action<IRoom, string> OnUpdateAction { get; set; }
}