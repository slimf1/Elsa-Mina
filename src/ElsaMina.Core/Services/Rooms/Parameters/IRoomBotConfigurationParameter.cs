﻿namespace ElsaMina.Core.Services.Rooms.Parameters;

public interface IRoomBotConfigurationParameter
{
    string Identifier { get; }
    string NameKey { get; }
    string DescriptionKey { get; }
    RoomBotConfigurationType Type { get; }
    string DefaultValue { get; }
    IEnumerable<EnumerationValue> PossibleValues { get; } // Only used for the enumeration type
    Action<IRoom, string> OnUpdateAction { get; }
}