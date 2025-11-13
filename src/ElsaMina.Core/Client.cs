using System.Net.WebSockets;
using System.Reactive.Linq;
using ElsaMina.Core.Services.Config;
using ElsaMina.Logging;
using Websocket.Client;

namespace ElsaMina.Core;

public class Client : IClient
{
    private static readonly TimeSpan RECONNECT_TIMEOUT = TimeSpan.FromSeconds(30);
    private readonly IConfiguration _configuration;
    private readonly WebsocketClient _websocketClient;
    private bool _disposed;

    public Client(IConfiguration configuration)
    {
        _configuration = configuration;

        _websocketClient = new WebsocketClient(Uri);
        _websocketClient.IsReconnectionEnabled = true;
        _websocketClient.ErrorReconnectTimeout = RECONNECT_TIMEOUT;
        _websocketClient.LostReconnectTimeout = RECONNECT_TIMEOUT;
    }

    private Uri Uri => new($"wss://{_configuration.Host}:{_configuration.Port}/showdown/websocket");

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

    public IObservable<ReconnectionInfo> ReconnectionHappened => _websocketClient.ReconnectionHappened;

    public bool IsConnected => _websocketClient.IsRunning;

    public async Task Connect()
    {
        Log.Information("Connecting to : {0}", _websocketClient.Url);
        await _websocketClient.StartOrFail();
    }

    public async Task Close()
    {
        Log.Information("Closing connection");
        await _websocketClient.StopOrFail(WebSocketCloseStatus.Empty, string.Empty);
    }

    public void Send(string message)
    {
        _websocketClient.Send(message);
    }

    public async Task SendAsync(string message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _websocketClient.SendInstant(message).WaitAsync(cancellationToken);
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