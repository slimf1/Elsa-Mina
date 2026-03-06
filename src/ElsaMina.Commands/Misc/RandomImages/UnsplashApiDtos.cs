using Newtonsoft.Json;

namespace ElsaMina.Commands.Misc.RandomImages;

public class UnsplashPhotoDto
{
    [JsonProperty("urls")]
    public UnsplashPhotoUrlsDto Urls { get; set; }
}

public class UnsplashPhotoUrlsDto
{
    [JsonProperty("regular")]
    public string Regular { get; set; }
}
