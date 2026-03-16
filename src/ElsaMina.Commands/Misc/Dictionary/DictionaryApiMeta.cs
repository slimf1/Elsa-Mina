using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Dictionary;

public class DictionaryApiMeta
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("uuid")]
    public string Uuid { get; set; }

    [JsonProperty("src")]
    public string Source { get; set; }

    [JsonProperty("section")]
    public string Section { get; set; }

    [JsonProperty("stems")]
    public List<string> Stems { get; set; }

    [JsonProperty("offensive")]
    public bool IsOffensive { get; set; }
}
