using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Bitcoin;

public class CoinDeskResponseDto
{
    [JsonProperty("bpi")]
    public IDictionary<string, BpiDto> Bpi { get; set; }
}