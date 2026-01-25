using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Wiki;

public class WikipediaApiSearchResponse
{
    [JsonProperty("batchcomplete")]
    public string BatchComplete { get; set; }

    [JsonProperty("continue")]
    public Continue Continue { get; set; }

    [JsonProperty("query")]
    public QueryPages Query { get; set; }
}

public class Continue
{
    [JsonProperty("gsroffset")]
    public int GsrOffset { get; set; }

    [JsonProperty("continue")]
    public string ContinueToken { get; set; }
}

public class QueryPages
{
    [JsonProperty("pages")]
    public Dictionary<string, WikiPage> Pages { get; set; }
}

public class WikiPage
{
    [JsonProperty("pageid")]
    public int PageId { get; set; }

    [JsonProperty("ns")]
    public int Namespace { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("pageprops")]
    public IDictionary<string, string> PageProps { get; set; }
}

public class WikipediaExtractResponse
{
    [JsonProperty("batchcomplete")]
    public string BatchComplete { get; set; }

    [JsonProperty("query")]
    public QueryWithExtract Query { get; set; }
}

public class QueryWithExtract
{
    [JsonProperty("normalized")]
    public List<NormalizedTitle> Normalized { get; set; }

    [JsonProperty("pages")]
    public Dictionary<string, WikiExtractPage> Pages { get; set; }
}

public class NormalizedTitle
{
    [JsonProperty("from")]
    public string From { get; set; }

    [JsonProperty("to")]
    public string To { get; set; }
}

public class WikiExtractPage
{
    [JsonProperty("pageid")]
    public int PageId { get; set; }

    [JsonProperty("ns")]
    public int Namespace { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("extract")]
    public string Extract { get; set; }

    [JsonProperty("thumbnail")]
    public Thumbnail Thumbnail { get; set; }
}

public class Thumbnail
{
    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }
}
