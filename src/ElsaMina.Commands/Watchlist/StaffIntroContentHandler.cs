using ElsaMina.Core.Handlers;

namespace ElsaMina.Commands.Watchlist;

public class StaffIntroContentHandler : Handler
{
    private readonly IWatchlistService _watchlistService;

    public StaffIntroContentHandler(IWatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (parts.Length < 3 || parts[1] != "raw")
        {
            return Task.CompletedTask;
        }

        var htmlContent = string.Join("|", parts[2..]);
        _watchlistService.HandleReceivedStaffIntro(roomId, htmlContent);
        return Task.CompletedTask;
    }
}
