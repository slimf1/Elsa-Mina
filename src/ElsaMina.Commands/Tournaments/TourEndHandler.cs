using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Tournaments;

public class TourEndHandler : Handler
{
    private readonly IBotDbContextFactory _botDbContextFactory;
    private readonly IRoomUserDataService _roomUserDataService;

    public TourEndHandler(IBotDbContextFactory botDbContextFactory, IRoomUserDataService roomUserDataService)
    {
        _botDbContextFactory = botDbContextFactory;
        _roomUserDataService = roomUserDataService;
    }

    public override async Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (parts.Length < 4 || parts[1] != "tournament" || parts[2] != "end")
        {
            return;
        }

        var result = TournamentHelper.ParseTourResults(parts[3]);
        if (result == null)
        {
            return;
        }

        try
        {
            await using var dbContext = await _botDbContextFactory.CreateDbContextAsync(cancellationToken);

            foreach (var player in result.Players)
            {
                var userId = player.ToLowerAlphaNum();
                await _roomUserDataService.GetOrCreateRoomSpecificUserDataAsync(roomId, userId, cancellationToken);

                var wonGamesInTournament = result.WinsCount.GetValueOrDefault(userId, 0);
                var playedGamesInTournament = wonGamesInTournament + (userId == result.Winner ? 0 : 1);

                var record = await dbContext.TournamentRecords.FindAsync([userId, roomId], cancellationToken);
                if (record == null)
                {
                    record = new TournamentRecord { UserId = userId, RoomId = roomId };
                    await dbContext.TournamentRecords.AddAsync(record, cancellationToken);
                }

                record.TournamentsEnteredCount++;
                record.WinsCount += userId == result.Winner ? 1 : 0;
                record.RunnerUpCount += userId == result.RunnerUp ? 1 : 0;
                record.ThirdPlaceCount += result.SemiFinalists.Contains(userId) ? 1 : 0;
                record.WonGames += wonGamesInTournament;
                record.PlayedGames += playedGamesInTournament;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving tournament results for room {RoomId}", roomId);
        }
    }
}