using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.RandomImages;

public class TenorResponseDto
{
    [JsonProperty("results")]
    public List<TenorResultDto> Results { get; set; }
}

public class TenorResultDto
{
    [JsonProperty("media_formats")]
    public Dictionary<string, TenorMediaDto> MediaFormats { get; set; }
}

public class TenorMediaDto
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("dims")]
    public int[] Dims { get; set; }
}
