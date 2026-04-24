using System.Collections.Concurrent;
using ElsaMina.Commands.Tournaments.Betting;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Utils;
using ElsaMina.Logging;
using Newtonsoft.Json;

namespace ElsaMina.Commands.Tournaments.Handlers;

public class TournamentBettingHandler : Handler
{
    private readonly ConcurrentDictionary<string, string[]> _pendingPlayers = new();
    private readonly ITournamentBettingService _tournamentBettingService;

    public TournamentBettingHandler(ITournamentBettingService tournamentBettingService)
    {
        _tournamentBettingService = tournamentBettingService;
    }

    public override IReadOnlySet<string> HandledMessageTypes => (HashSet<string>)["tournament"];

    public override async Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (roomId == null || parts.Length < 3)
        {
            return;
        }

        if (parts[2] == "update" && parts.Length >= 4)
        {
            var update = JsonConvert.DeserializeObject<TournamentUpdate>(parts[3]);
            var incomingUsers = update?.BracketData?.Users;
            if (!incomingUsers.IsNullOrEmpty())
            {
                _pendingPlayers[roomId] = incomingUsers;
            }
        }
        else if (parts[2] == "start")
        {
            if (_pendingPlayers.TryGetValue(roomId, out var users))
            {
                _pendingPlayers.Remove(roomId, out _);
                await _tournamentBettingService.AnnounceBetsAsync(users, roomId, cancellationToken);
            }
        }
        else if (parts[2] == "forceend")
        {
            _pendingPlayers.Remove(roomId, out _);
            await _tournamentBettingService.ReturnBetsAsync(roomId, cancellationToken);
        }
        else if (parts[2] == "end" && parts.Length >= 4)
        {
            _pendingPlayers.Remove(roomId, out _);

            try
            {
                var result = TournamentHelper.ParseTourResults(parts[3]);
                if (result?.Winner != null)
                {
                    await _tournamentBettingService.ResolveBetsAsync(result.Winner, roomId, cancellationToken);
                }
                else
                {
                    await _tournamentBettingService.ReturnBetsAsync(roomId, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error resolving bets for room {RoomId}", roomId);
                await _tournamentBettingService.ReturnBetsAsync(roomId, cancellationToken);
            }
        }
    }
}