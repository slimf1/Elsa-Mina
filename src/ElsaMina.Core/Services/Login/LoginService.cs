using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Core.Services.Login;

public class LoginService : ILoginService
{
    public const string LOGIN_URL = "https://play.pokemonshowdown.com/action.php";

    private readonly IHttpService _httpService;
    private readonly IConfigurationManager _configurationManager;

    public LoginService(IHttpService httpService, IConfigurationManager configurationManager)
    {
        _httpService = httpService;
        _configurationManager = configurationManager;
    }

    public async Task<LoginResponseDto> Login(string challstr)
    {
        var form = new Dictionary<string, string>
        {
            ["challstr"] = challstr,
            ["name"] = _configurationManager.Configuration.Name,
            ["pass"] = _configurationManager.Configuration.Password,
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