namespace ElsaMina.Core;

public interface IBot : IDisposable
{
    Task HandleReceivedMessageAsync(string message);
    void Send(string message);
    void Say(string roomId, string message);
    Task StartAsync();
    void OnReconnect();
    void OnDisconnect();
    void OnExit();
    TimeSpan UpTime { get; }
}