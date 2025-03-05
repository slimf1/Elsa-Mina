namespace ElsaMina.Core.Handlers;

public interface IHandlerManager
{
    bool IsInitialized { get; }
    void Initialize();
    Task HandleMessage(string[] parts, string roomId = null);
}