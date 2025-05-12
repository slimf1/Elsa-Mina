using Websocket.Client;

namespace ElsaMina.Core;

public interface IClient : IDisposable
{
    Task Connect();
    void Send(string message);
    Task SendAsync(string message, CancellationToken cancellationToken);
    Task Close();
    IObservable<string> MessageReceived { get; }
    IObservable<string> DisconnectionHappened { get; }
    IObservable<ReconnectionInfo> ReconnectionHappened { get; }
    bool IsConnected { get; }
}