namespace ElsaMina.Core.Handlers;

public interface IHandlerManager
{
    bool IsInitialized { get; }
    Task Initialize();
    Task HandleMessage(string[] parts, string roomId = null);
}