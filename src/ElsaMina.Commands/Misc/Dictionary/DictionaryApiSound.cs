using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Dictionary;

public class DictionaryApiSound
{
    [JsonProperty("audio")]
    public string Audio { get; set; }
}
