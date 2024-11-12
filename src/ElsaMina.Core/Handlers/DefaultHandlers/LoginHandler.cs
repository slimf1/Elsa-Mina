using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.System;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public class LoginHandler : Handler
{
    private readonly ILoginService _loginService;
    private readonly IConfigurationManager _configurationManager;
    private readonly ISystemService _systemService;
    private readonly IClient _client;

    public LoginHandler(ILoginService loginService,
        IConfigurationManager configurationManager,
        ISystemService systemService,
        IClient client)
    {
        _loginService = loginService;
        _configurationManager = configurationManager;
        _systemService = systemService;
        _client = client;
    }

    public override string Identifier => nameof(LoginHandler);

    public override async Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (parts.Length > 2 && parts[1] == "challstr")
        {
            Logger.Information("Logging in...");
            var nonce = string.Join("|", parts[2..]);
            var response = await _loginService.Login(nonce);

            if (response?.CurrentUser == null ||
                _configurationManager.Configuration.Name.ToLowerAlphaNum() != response.CurrentUser.UserId)
            {
                Logger.Error("Login failed. Check password validity. Exiting");
                _systemService.Kill();
                return;
            }

            _client.Send($"|/trn {response.CurrentUser.Username},0,{response.Assertion}");
        }
    }
}