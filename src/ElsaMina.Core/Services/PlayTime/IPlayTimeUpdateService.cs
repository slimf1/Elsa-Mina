namespace ElsaMina.Core.Services.PlayTime;

public interface IPlayTimeUpdateService : IDisposable
{
    void Initialize();
    Task ProcessPendingPlayTimeUpdatesAsync();
    Task WaitForPlayTimeUpdatesAsync(CancellationToken cancellationToken = default);
}
