using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Core.Services.Login;

public class LoginService : ILoginService
{
    public const string LOGIN_URL = "https://play.pokemonshowdown.com/action.php";

    private readonly IHttpService _httpService;
    private readonly IConfiguration _configuration;

    public LoginService(IHttpService httpService, IConfiguration configuration)
    {
        _httpService = httpService;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Login(string challstr)
    {
        var form = new Dictionary<string, string>
        {
            ["challstr"] = challstr,
            ["name"] = _configuration.Name,
            ["pass"] = _configuration.Password,
            ["act"] = "login"
        };

        try
        {
            var response = await _httpService.PostUrlEncodedFormAsync<LoginResponseDto>(LOGIN_URL, form, true);
            return response.Data;
        }
        catch (HttpException exception)
        {
            Log.Error("Login failed with status code {0} : {1}",
                exception.StatusCode, exception.ResponseContent);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Login failed");
        }

        return null;
    }
}