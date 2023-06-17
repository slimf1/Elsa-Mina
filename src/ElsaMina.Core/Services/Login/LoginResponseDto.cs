using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Login;

public class LoginResponseDto
{
    [JsonProperty("actionsuccess")]
    public bool IsSuccess { get; set; }
    [JsonProperty("assertion")]
    public string Assertion { get; set; }
    [JsonProperty("curuser")]
    public CurrentUserDto CurrentUser { get; set; }
}