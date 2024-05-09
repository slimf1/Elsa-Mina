using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Models;

public interface IRoomBotConfigurationParameter
{
    string Identifier { get; set; }
    string NameKey { get; set; }
    string DescriptionKey { get; set; }
    RoomBotConfigurationType Type { get; set; }
    string DefaultValue { get; set; }
    IEnumerable<EnumerationValue> PossibleValues { get; set; } // Only used for the enumeration type
    Action<IRoom, string> OnUpdateAction { get; set; }
}