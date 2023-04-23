namespace ElsaMina.Core.Client;

public interface IClient : IDisposable
{
    Task Connect();
    void Send(string message);
    IObservable<string> MessageReceived { get; }
}