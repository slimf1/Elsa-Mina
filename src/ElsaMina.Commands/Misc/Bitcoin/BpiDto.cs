using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Bitcoin;

public class BpiDto
{
    [JsonProperty("code")]
    public string Code { get; set; }
    [JsonProperty("rate_float")]
    public double Rate { get; set; }
}
