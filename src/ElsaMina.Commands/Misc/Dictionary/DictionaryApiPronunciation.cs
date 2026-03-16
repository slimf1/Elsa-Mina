using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Dictionary;

public class DictionaryApiPronunciation
{
    [JsonProperty("mw")]
    public string MerriamWebster { get; set; }

    [JsonProperty("sound")]
    public DictionaryApiSound Sound { get; set; }
}
