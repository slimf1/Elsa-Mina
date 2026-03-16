using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Dictionary;

public class DictionaryApiUndefinedRunOn
{
    [JsonProperty("ure")]
    public string UndefinedRunOn { get; set; }

    [JsonProperty("prs")]
    public List<DictionaryApiPronunciation> Pronunciations { get; set; }

    [JsonProperty("fl")]
    public string PartOfSpeech { get; set; }
}
