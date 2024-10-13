using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Facts;

public class FactDto
{
    [JsonProperty("text")]
    public string Text { get; set; }
}