namespace ElsaMina.Commands.Misc.Dailymotion;

using Newtonsoft.Json;
using System.Collections.Generic;

public class VideoListResponse
{
    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonProperty("limit")]
    public int Limit { get; set; }

    [JsonProperty("explicit")]
    public bool Explicit { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("list")]
    public List<VideoItem> List { get; set; } = [];
}

public class VideoItem
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("views_total")]
    public int ViewsTotal { get; set; }

    [JsonProperty("likes_total")]
    public int LikesTotal { get; set; }

    [JsonProperty("explicit")]
    public bool Explicit { get; set; }
}