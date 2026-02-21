using Newtonsoft.Json;

namespace ElsaMina.Core.Services.BattleTracker;

internal class RoomListQueryResponseDto
{
    [JsonProperty("rooms")]
    public IDictionary<string, ActiveBattleDto> Rooms { get; set; }
}
