using ElsaMina.Core.Models;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoomConfigurationParametersFactory
{
    IReadOnlyDictionary<string, IRoomBotConfigurationParameter> GetParameters();
}