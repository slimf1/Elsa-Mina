using System.Collections.Concurrent;
using System.Globalization;
using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Tournaments.Betting;

public class TournamentBettingService : ITournamentBettingService
{
    private record ActiveBet(string BettorId, string TargetPlayerId);

    private readonly ConcurrentDictionary<string, HashSet<string>> _activePlayers = new();
    private readonly ConcurrentDictionary<string, List<ActiveBet>> _activeBets = new();

    private readonly IBot _bot;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;

    public TournamentBettingService(IBot bot, ITemplatesManager templatesManager, IConfiguration configuration)
    {
        _bot = bot;
        _templatesManager = templatesManager;
        _configuration = configuration;
    }

    public async Task AnnounceBetsAsync(string[] players, string roomId,
        CancellationToken cancellationToken = default)
    {
        _activePlayers[roomId] = players.Select(p => p.ToLowerAlphaNum()).ToHashSet();
        _activeBets[roomId] = [];

        var viewModel = new BettingAnnouncementViewModel
        {
            Culture = CultureInfo.InvariantCulture,
            Players = players,
            BotName = _configuration.Name,
            Trigger = _configuration.Trigger,
            RoomId = roomId
        };
        var template = await _templatesManager.GetTemplateAsync("Tournaments/Betting/BettingAnnouncement", viewModel);
        _bot.Say(roomId, $"/addhtmlbox {template.RemoveNewlines()}");
    }

    public Task<BetPlacementError> PlaceBetAsync(string bettorId, string targetPlayerId, string roomId,
        CancellationToken cancellationToken = default)
    {
        if (!_activePlayers.TryGetValue(roomId, out var players) || !_activeBets.TryGetValue(roomId, out var bets))
        {
            return Task.FromResult(BetPlacementError.NoBettingSession);
        }

        if (!players.Contains(targetPlayerId))
        {
            return Task.FromResult(BetPlacementError.InvalidPlayer);
        }

        if (bets.Any(bet => bet.BettorId == bettorId && bet.TargetPlayerId == targetPlayerId))
        {
            return Task.FromResult(BetPlacementError.AlreadyBetOnPlayer);
        }

        bets.Add(new ActiveBet(bettorId, targetPlayerId));
        return Task.FromResult(BetPlacementError.Success);
    }

    public Task<int> CancelBetAsync(string bettorId, string roomId, string targetPlayerId = null,
        CancellationToken cancellationToken = default)
    {
        if (!_activeBets.TryGetValue(roomId, out var bets))
        {
            return Task.FromResult(0);
        }

        var toCancel = targetPlayerId != null
            ? bets.Where(bet => bet.BettorId == bettorId && bet.TargetPlayerId == targetPlayerId).ToList()
            : bets.Where(bet => bet.BettorId == bettorId).ToList();

        foreach (var bet in toCancel)
        {
            bets.Remove(bet);
        }

        return Task.FromResult(toCancel.Count);
    }

    public Task ResolveBetsAsync(string winnerId, string roomId, CancellationToken cancellationToken = default)
    {
        if (!_activeBets.TryGetValue(roomId, out var bets) || bets.Count == 0)
        {
            CleanUp(roomId);
            return Task.CompletedTask;
        }

        var winners = bets
            .Where(bet => bet.TargetPlayerId == winnerId)
            .Select(bet => bet.BettorId)
            .Distinct()
            .ToList();

        if (winners.Count > 0)
        {
            var names = string.Join(", ", winners);
            _bot.Say(roomId, $"/addhtmlbox <div>🏆 <b>{winnerId}</b> won! Correct guesses: <b>{names}</b></div>");
        }
        else
        {
            _bot.Say(roomId, $"/addhtmlbox <div>🏆 <b>{winnerId}</b> won! Nobody guessed correctly.</div>");
        }

        CleanUp(roomId);
        return Task.CompletedTask;
    }

    public Task ReturnBetsAsync(string roomId, CancellationToken cancellationToken = default)
    {
        CleanUp(roomId);
        return Task.CompletedTask;
    }

    private void CleanUp(string roomId)
    {
        _activePlayers.TryRemove(roomId, out _);
        _activeBets.TryRemove(roomId, out _);
    }
}
