using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Login;

public class CurrentUserDto
{
    [JsonProperty("loggedin")]
    public bool IsLoggedIn { get; set; }
    [JsonProperty("userid")]
    public string UserId { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
}