using Newtonsoft.Json;

namespace ElsaMina.Core.Services.BattleTracker;

public class ActiveBattleDto
{
    public string RoomId { get; set; }

    [JsonProperty("p1")]
    public string Player1 { get; set; }

    [JsonProperty("p2")]
    public string Player2 { get; set; }

    [JsonProperty("minElo")]
    public int? MinElo { get; set; }
}
