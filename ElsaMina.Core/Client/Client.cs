using System.Net.WebSockets;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Client;

public class Client : IClient
{
    private readonly IConfigurationService _configurationService;
    
    private WebSocket? _webSocket;

    public Client(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public void Connect()
    {
        Console.Write(_configurationService.Configuration?.Password);
    }

    public void Send(string message)
    {
    }
}