namespace ElsaMina.Core.Services.Rooms;

public interface IPlayTimeUpdateService : IDisposable
{
    void Initialize();
    Task ProcessPendingPlayTimeUpdatesAsync();
    Task WaitForPlayTimeUpdatesAsync(CancellationToken cancellationToken = default);
}
