using Newtonsoft.Json;

namespace ElsaMina.Commands.Arcade;

public class ArcadeEventWebhookBody
{
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }
    [JsonProperty("embeds")]
    public List<ArcadeEventWebhookEmbed> Embeds { get; set; }
}