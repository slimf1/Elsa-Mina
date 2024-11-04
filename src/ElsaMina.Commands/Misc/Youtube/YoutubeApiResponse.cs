using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Youtube;

public class PageInfo
{
    [JsonProperty("totalResults")]
    public int TotalResults { get; set; }

    [JsonProperty("resultsPerPage")]
    public int ResultsPerPage { get; set; }
}

public class VideoId
{
    [JsonProperty("kind")]
    public string Kind { get; set; }

    [JsonProperty("videoId")]
    public string VideoIdValue { get; set; }
}

public class Thumbnail
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }
}

public class Thumbnails
{
    [JsonProperty("default")]
    public Thumbnail Default { get; set; }

    [JsonProperty("medium")]
    public Thumbnail Medium { get; set; }

    [JsonProperty("high")]
    public Thumbnail High { get; set; }
}

public class Snippet
{
    [JsonProperty("publishedAt")]
    public string PublishedAt { get; set; }

    [JsonProperty("channelId")]
    public string ChannelId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("thumbnails")]
    public Thumbnails Thumbnails { get; set; }

    [JsonProperty("channelTitle")]
    public string ChannelTitle { get; set; }

    [JsonProperty("liveBroadcastContent")]
    public string LiveBroadcastContent { get; set; }

    [JsonProperty("publishTime")]
    public string PublishTime { get; set; }
}

public class SearchResultItem
{
    [JsonProperty("kind")]
    public string Kind { get; set; }

    [JsonProperty("etag")]
    public string ETag { get; set; }

    [JsonProperty("id")]
    public VideoId Id { get; set; }

    [JsonProperty("snippet")]
    public Snippet Snippet { get; set; }
}

public class YouTubeSearchResponse
{
    [JsonProperty("kind")]
    public string Kind { get; set; }

    [JsonProperty("etag")]
    public string ETag { get; set; }

    [JsonProperty("nextPageToken")]
    public string NextPageToken { get; set; }

    [JsonProperty("regionCode")]
    public string RegionCode { get; set; }

    [JsonProperty("pageInfo")]
    public PageInfo PageInfo { get; set; }

    [JsonProperty("items")]
    public List<SearchResultItem> Items { get; set; }
}
