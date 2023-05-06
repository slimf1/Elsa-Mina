using System.Reactive.Linq;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using Websocket.Client;

namespace ElsaMina.Core.Client;

public class Client : IClient
{
    private readonly IConfigurationManager _configurationManager;

    private readonly WebsocketClient _websocketClient;
    private bool _disposed;
    
    private IConfiguration Conf => _configurationManager.Configuration;

    public Client(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;

        _websocketClient = new WebsocketClient(new Uri($"ws://{Conf.Host}:{Conf.Port}/showdown/websocket"));
    }

    public async Task Connect()
    {
        await _websocketClient.Start();
    }
    
    public IObservable<string> MessageReceived => _websocketClient
        .MessageReceived
        .Select(message => message.Text);

    public void Send(string message)
    {
        _websocketClient.Send(message);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _websocketClient.Dispose();
        }

        _disposed = true;
    }
}