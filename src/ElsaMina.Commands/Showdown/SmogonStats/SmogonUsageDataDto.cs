using Newtonsoft.Json;

namespace ElsaMina.Commands.Showdown.SmogonStats;

public class SmogonUsageDataDto
{
    [JsonProperty("info")]
    public SmogonUsageInfoDto Info { get; set; }

    [JsonProperty("data")]
    public Dictionary<string, SmogonPokemonUsageDataDto> Data { get; set; }
}
