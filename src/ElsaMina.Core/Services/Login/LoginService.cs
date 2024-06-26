﻿using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Core.Services.Login;

public class LoginService : ILoginService
{
    private const string LOGIN_URL = "http://play.pokemonshowdown.com/action.php";

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
        // todo : auto retry ?
        try
        {
            return await _httpService.PostUrlEncodedForm<LoginResponseDto>(LOGIN_URL, form, true);
        }
        catch (HttpException exception)
        {
            Logger.Current.Error("Login failed with status code {0} : {1}",
                exception.StatusCode, exception.ResponseContent);
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "Login failed");
        }

        return null;
    }
}