using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.Genius;

public class GeniusSearchResult
{
    [JsonProperty("meta")]
    public Meta Meta { get; set; }

    [JsonProperty("response")]
    public Response Response { get; set; }
}

public class Meta
{
    [JsonProperty("status")]
    public int Status { get; set; }
}

public class Response
{
    [JsonProperty("hits")]
    public List<Hit> Hits { get; set; }
}

public class Hit
{
    [JsonProperty("highlights")]
    public List<object> Highlights { get; set; }

    [JsonProperty("index")]
    public string Index { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("result")]
    public Result Result { get; set; }
}

public class Result
{
    [JsonProperty("annotation_count")]
    public int AnnotationCount { get; set; }

    [JsonProperty("api_path")]
    public string ApiPath { get; set; }

    [JsonProperty("artist_names")]
    public string ArtistNames { get; set; }

    [JsonProperty("full_title")]
    public string FullTitle { get; set; }

    [JsonProperty("header_image_thumbnail_url")]
    public string HeaderImageThumbnailUrl { get; set; }

    [JsonProperty("header_image_url")]
    public string HeaderImageUrl { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("lyrics_owner_id")]
    public long LyricsOwnerId { get; set; }

    [JsonProperty("lyrics_state")]
    public string LyricsState { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }

    [JsonProperty("primary_artist_names")]
    public string PrimaryArtistNames { get; set; }

    [JsonProperty("pyongs_count")]
    public int? PyongsCount { get; set; }

    [JsonProperty("relationships_index_url")]
    public string RelationshipsIndexUrl { get; set; }

    [JsonProperty("release_date_components")]
    public ReleaseDateComponents ReleaseDateComponents { get; set; }

    [JsonProperty("release_date_for_display")]
    public string ReleaseDateForDisplay { get; set; }

    [JsonProperty("release_date_with_abbreviated_month_for_display")]
    public string ReleaseDateWithAbbreviatedMonthForDisplay { get; set; }

    [JsonProperty("song_art_image_thumbnail_url")]
    public string SongArtImageThumbnailUrl { get; set; }

    [JsonProperty("song_art_image_url")]
    public string SongArtImageUrl { get; set; }

    [JsonProperty("stats")]
    public Stats Stats { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("title_with_featured")]
    public string TitleWithFeatured { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("featured_artists")]
    public List<Artist> FeaturedArtists { get; set; }

    [JsonProperty("primary_artist")]
    public Artist PrimaryArtist { get; set; }

    [JsonProperty("primary_artists")]
    public List<Artist> PrimaryArtists { get; set; }
}

public class ReleaseDateComponents
{
    [JsonProperty("year")]
    public int? Year { get; set; }

    [JsonProperty("month")]
    public int? Month { get; set; }

    [JsonProperty("day")]
    public int? Day { get; set; }
}

public class Stats
{
    [JsonProperty("unreviewed_annotations")]
    public int UnreviewedAnnotations { get; set; }

    [JsonProperty("hot")]
    public bool Hot { get; set; }

    [JsonProperty("pageviews")]
    public int? Pageviews { get; set; }
}

public class Artist
{
    [JsonProperty("api_path")]
    public string ApiPath { get; set; }

    [JsonProperty("header_image_url")]
    public string HeaderImageUrl { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }

    [JsonProperty("is_meme_verified")]
    public bool IsMemeVerified { get; set; }

    [JsonProperty("is_verified")]
    public bool IsVerified { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("iq")]
    public int? Iq { get; set; }
}
