namespace ElsaMina.Core;

public interface IClient : IDisposable
{
    Task Connect();
    void Send(string message);
    Task Close();
    IObservable<string> MessageReceived { get; }
    IObservable<string> DisconnectionHappened { get; }
    bool IsConnected { get; }
}