namespace ElsaMina.Core.Bot;

public interface IBot : IDisposable
{
    Task HandleReceivedMessage(string message);
    Task Start();
    void Send(string message);
    void Say(string roomId, string message);
}