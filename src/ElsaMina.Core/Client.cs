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

    public Client(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;

        _websocketClient = new WebsocketClient(new Uri($"wss://{Conf.Host}:{Conf.Port}/showdown/websocket"));
        _websocketClient.IsReconnectionEnabled = false;
    }

    private IConfiguration Conf => _configurationManager.Configuration;

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

    public async Task Connect()
    {
        Logger.Information("Connecting to : {0}", _websocketClient.Url);
        await _websocketClient.StartOrFail();
    }

    public async Task Close()
    {
        Logger.Information("Closing connection");
        await _websocketClient.Stop(WebSocketCloseStatus.Empty, string.Empty);
    }

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

    ~Client()
    {
        Dispose(false);
    }
}