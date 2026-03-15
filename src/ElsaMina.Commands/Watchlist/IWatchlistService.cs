namespace ElsaMina.Commands.Watchlist;

public interface IWatchlistService
{
    Task<Dictionary<string, string>> GetWatchlistAsync(string roomId, CancellationToken cancellationToken = default);
    Task AddToWatchlistAsync(string roomId, string user, string rank, CancellationToken cancellationToken = default);
    Task<bool> RemoveFromWatchlistAsync(string roomId, string user, string rank, CancellationToken cancellationToken = default);
    Task FetchAndUpdateStaffIntroAsync(string roomId, CancellationToken cancellationToken = default);
    void HandleReceivedStaffIntro(string roomId, string htmlContent);
    Task SendDiscordNotificationAsync(string roomId, string message, CancellationToken cancellationToken = default);
}
