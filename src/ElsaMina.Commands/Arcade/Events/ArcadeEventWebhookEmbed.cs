using Newtonsoft.Json;

namespace ElsaMina.Commands.Arcade.Events;

public class ArcadeEventWebhookEmbed
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("color")]
    public int Color { get; set; }
}
