using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Youtube;

public class YouTubeVideoListResponse
{
    [JsonProperty("items")]
    public List<YouTubeVideoItem> Items { get; set; }
}
