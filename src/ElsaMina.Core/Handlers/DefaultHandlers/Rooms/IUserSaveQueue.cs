namespace ElsaMina.Core.Handlers.DefaultHandlers.Rooms;

public interface IUserSaveQueue : IAsyncDisposable
{
    void Enqueue(string userName);
    Task FlushAsync(CancellationToken cancellationToken);
    Task WaitForFlushAsync(CancellationToken cancellationToken = default);
    Task AcquireLockAsync(CancellationToken cancellationToken = default);
    void ReleaseLock();
}