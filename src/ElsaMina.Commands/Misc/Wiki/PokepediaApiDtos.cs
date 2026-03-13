using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Wiki;

public class PokepediaParseResponse
{
    [JsonProperty("parse")]
    public PokepediaParseResult Parse { get; set; }
}

public class PokepediaParseResult
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("pageid")]
    public int PageId { get; set; }

    [JsonProperty("wikitext")]
    public PokepediaWikitext Wikitext { get; set; }
}

public class PokepediaWikitext
{
    [JsonProperty("*")]
    public string Content { get; set; }
}
