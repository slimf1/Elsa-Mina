using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Dictionary;

public class DictionaryApiEntry
{
    [JsonProperty("meta")]
    public DictionaryApiMeta Meta { get; set; }

    [JsonProperty("hwi")]
    public DictionaryApiHeadwordInfo HeadwordInfo { get; set; }

    [JsonProperty("fl")]
    public string PartOfSpeech { get; set; }

    [JsonProperty("def")]
    public List<DictionaryApiDefinition> Definitions { get; set; }

    [JsonProperty("uros")]
    public List<DictionaryApiUndefinedRunOn> UndefinedRunOns { get; set; }

    [JsonProperty("shortdef")]
    public List<string> ShortDefinitions { get; set; }
}
