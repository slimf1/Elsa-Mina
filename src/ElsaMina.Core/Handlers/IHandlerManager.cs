namespace ElsaMina.Core.Handlers;

public interface IHandlerManager
{
    bool IsInitialized { get; }
    void Initialize();
    Task HandleMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default);
}