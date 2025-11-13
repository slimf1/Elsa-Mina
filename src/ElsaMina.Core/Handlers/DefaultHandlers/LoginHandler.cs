using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Login;
using ElsaMina.Core.Services.System;
using ElsaMina.Core.Utils;
using ElsaMina.Logging;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public class LoginHandler : Handler
{
    private readonly ILoginService _loginService;
    private readonly IConfiguration _configuration;
    private readonly ISystemService _systemService;
    private readonly IClient _client;

    public LoginHandler(ILoginService loginService,
        IConfiguration configuration,
        ISystemService systemService,
        IClient client)
    {
        _loginService = loginService;
        _configuration = configuration;
        _systemService = systemService;
        _client = client;
    }

    public override async Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        if (parts.Length <= 2 || parts[1] != "challstr")
        {
            return;
        }

        Log.Information("Logging in...");
        var nonce = string.Join("|", parts[2..]);
        var response = await _loginService.Login(nonce);

        if (response?.CurrentUser == null ||
            _configuration.Name.ToLowerAlphaNum() != response.CurrentUser.UserId)
        {
            Log.Error("Login failed. Check password validity. Exiting");
            _systemService.Kill();
            return;
        }

        _client.Send($"|/trn {response.CurrentUser.Username},0,{response.Assertion}");
    }
}