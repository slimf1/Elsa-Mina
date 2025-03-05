namespace ElsaMina.Core;

public interface IBot : IDisposable
{
    Task HandleReceivedMessage(string message);
    void Send(string message);
    void Say(string roomId, string message);
    Task Connect();
    void OnStart();
    void OnReset();
}