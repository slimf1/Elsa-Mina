using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Youtube;

public class YouTubeVideoItem
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("snippet")]
    public Snippet Snippet { get; set; }
}
