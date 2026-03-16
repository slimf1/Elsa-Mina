using ElsaMina.Core.Services.Login;
using ElsaMina.Logging;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public class LoginHandler : Handler
{
    private readonly ILoginService _loginService;
    private readonly IClient _client;

    public LoginHandler(ILoginService loginService, IClient client)
    {
        _loginService = loginService;
        _client = client;
    }

    public override async Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        if (parts.Length <= 2 || parts[1] != "challstr")
        {
            return;
        }

        var nonce = string.Join("|", parts[2..]);
        var response = await _loginService.Login(nonce, cancellationToken);
        Log.Information("Logged in as {0}", response.CurrentUser.Username);
        _client.Send($"|/trn {response.CurrentUser.Username},0,{response.Assertion}");
    }
}
