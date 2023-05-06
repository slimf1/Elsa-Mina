namespace ElsaMina.Core.Bot;

public interface IBot : IDisposable
{
    IEnumerable<string> Formats { get; }
    Task HandleReceivedMessage(string message);

    Task Start();
    void Send(string message);
    void Say(string roomId, string message);
}