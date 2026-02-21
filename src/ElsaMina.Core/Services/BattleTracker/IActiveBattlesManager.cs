namespace ElsaMina.Core.Services.BattleTracker;

public interface IActiveBattlesManager
{
    Task<IReadOnlyCollection<ActiveBattleDto>> GetActiveBattlesAsync(string format, int minimumElo = 0,
        string prefixFilter = "", CancellationToken cancellationToken = default);

    void HandleReceivedRoomList(string message);
}