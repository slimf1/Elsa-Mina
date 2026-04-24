using System.Collections.Concurrent;
using System.Globalization;
using ElsaMina.Core;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Tournaments.Betting;

public class TournamentBettingService : ITournamentBettingService
{
    private record ActiveBet(string BettorId, string TargetPlayerId);

    private const int BETTING_WINDOW_SECONDS = 30;

    private readonly ConcurrentDictionary<string, HashSet<string>> _activePlayers = new();
    private readonly ConcurrentDictionary<string, List<ActiveBet>> _activeBets = new();
    private readonly ConcurrentDictionary<string, DateTimeOffset> _closingTimes = new();
    private readonly CultureInfo _defaultCulture;

    private readonly IBot _bot;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;
    private readonly IClockService _clockService;

    public TournamentBettingService(IBot bot, ITemplatesManager templatesManager, IConfiguration configuration,
        IResourcesService resourcesService, IRoomsManager roomsManager, IClockService clockService)
    {
        _bot = bot;
        _templatesManager = templatesManager;
        _configuration = configuration;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
        _clockService = clockService;
        
        _defaultCulture = new CultureInfo(configuration.DefaultLocaleCode);
    }

    public async Task AnnounceBetsAsync(string[] players, string roomId,
        CancellationToken cancellationToken = default)
    {
        _activePlayers[roomId] = players.Select(p => p.ToLowerAlphaNum()).ToHashSet();
        _activeBets[roomId] = [];
        _closingTimes[roomId] = _clockService.CurrentUtcDateTimeOffset.AddSeconds(BETTING_WINDOW_SECONDS);
        
        var culture = _roomsManager.GetRoom(roomId)?.Culture;
        var viewModel = new BettingAnnouncementViewModel
        {
            Culture = culture ?? _defaultCulture,
            Players = players,
            BotName = _configuration.Name,
            Trigger = _configuration.Trigger,
            RoomId = roomId,
            SecondsToClose = BETTING_WINDOW_SECONDS
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

        if (_closingTimes.TryGetValue(roomId, out var closingTime) && _clockService.CurrentUtcDateTimeOffset > closingTime)
        {
            return Task.FromResult(BetPlacementError.BettingClosed);
        }

        if (!players.Contains(targetPlayerId))
        {
            return Task.FromResult(BetPlacementError.InvalidPlayer);
        }

        if (bets.Any(bet => bet.BettorId == bettorId))
        {
            return Task.FromResult(BetPlacementError.AlreadyBet);
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

        var culture = _roomsManager.GetRoom(roomId)?.Culture;

        var correctBettors = bets
            .Where(bet => bet.TargetPlayerId == winnerId)
            .Select(bet => bet.BettorId)
            .Distinct()
            .ToList();

        string message;
        if (correctBettors.Count > 0)
        {
            var names = string.Join(", ", correctBettors);
            message = string.Format(
                _resourcesService.GetString("bet_resolution_correct_guesses", culture), winnerId, names);
        }
        else
        {
            message = string.Format(
                _resourcesService.GetString("bet_resolution_nobody_correct", culture), winnerId);
        }

        _bot.Say(roomId, $"/addhtmlbox <div>{message}</div>");
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
        _closingTimes.TryRemove(roomId, out _);
    }
}