namespace ElsaMina.Commands.Development.LagTest;

public interface ILagTestManager
{
    Task<TimeSpan> StartLagTestAsync(string roomId, CancellationToken cancellationToken = default);
    void HandleEcho(string roomId);
}
