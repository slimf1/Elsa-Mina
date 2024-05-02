using System.Net.WebSockets;
using System.Reactive.Linq;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using Websocket.Client;

namespace ElsaMina.Core;

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
        _websocketClient.IsReconnectionEnabled = false;
    }

    public async Task Connect()
    {
        Logger.Current.Information("Connecting to : {0}", _websocketClient.Url);
        await _websocketClient.Start();
    }
    
    public async Task Close()
    {
        Logger.Current.Information("Closing connection");
        await _websocketClient.Stop(WebSocketCloseStatus.Empty, string.Empty);
    }
    
    public void Send(string message)
    {
        _websocketClient.Send(message);
    }

    public IObservable<string> MessageReceived => _websocketClient
        .MessageReceived
        .Select(message => message.Text);

    public IObservable<string> DisconnectionHappened => _websocketClient
        .DisconnectionHappened
        .Select(disconnectionInfo =>
        {
            var exception = disconnectionInfo.Exception;
            if (exception != null)
            {
                return $"{exception.Message}\n{exception.StackTrace}";
            }

            return disconnectionInfo.CloseStatus?.ToString() ?? disconnectionInfo.CloseStatusDescription;
        });

    public bool IsConnected => _websocketClient.IsRunning;

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

    ~Client()
    {
        Dispose(false);
    }
}