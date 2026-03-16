using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Dictionary;

public class DictionaryApiHeadwordInfo
{
    [JsonProperty("hw")]
    public string Headword { get; set; }

    [JsonProperty("prs")]
    public List<DictionaryApiPronunciation> Pronunciations { get; set; }
}
