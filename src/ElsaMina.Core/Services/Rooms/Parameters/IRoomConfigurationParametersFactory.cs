namespace ElsaMina.Core.Services.Rooms.Parameters;

public interface IRoomConfigurationParametersFactory
{
    IReadOnlyDictionary<string, IRoomBotConfigurationParameter> GetParameters();
}