namespace ElsaMina.Core.Client;

public interface IClient
{
    Task Connect();
    void Send(string message);
    IObservable<string> MessageReceived { get; }
}