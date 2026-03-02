using System.Collections.Concurrent;
using ElsaMina.Core;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Arcade.Inscriptions;

public class ArcadeInscriptionsManager : IArcadeInscriptionsManager
{
    private readonly ConcurrentDictionary<string, ArcadeRoomState> _roomStates = new();
    private readonly IBot _bot;
    private readonly IRoomsManager _roomsManager;
    private readonly IClockService _clockService;

    public ArcadeInscriptionsManager(IBot bot, IRoomsManager roomsManager, IClockService clockService)
    {
        _bot = bot;
        _roomsManager = roomsManager;
        _clockService = clockService;
    }

    public bool TryGetState(string roomId, out ArcadeRoomState state)
    {
        return _roomStates.TryGetValue(roomId, out state);
    }

    public bool HasActiveInscriptions(string roomId)
    {
        return _roomStates.TryGetValue(roomId, out var state) && state.IsActive;
    }

    public ArcadeRoomState InitInscriptions(string roomId, string title)
    {
        var state = new ArcadeRoomState
        {
            IsActive = true,
            Title = title
        };
        _roomStates[roomId] = state;
        return state;
    }

    public void StopInscriptions(string roomId)
    {
        CancelTimer(roomId);
        if (_roomStates.TryGetValue(roomId, out var state))
        {
            state.IsActive = false;
        }
    }

    public void StartTimer(string roomId, int minutes)
    {
        if (!_roomStates.TryGetValue(roomId, out var state))
        {
            return;
        }

        CancelTimer(roomId);

        var cancellationTokenSource = new CancellationTokenSource();
        state.TimerCts = cancellationTokenSource;
        state.TimerEnd = _clockService.CurrentUtcDateTimeOffset.AddMinutes(minutes);

        _ = RunTimerAsync(roomId, state, minutes, cancellationTokenSource.Token);
    }

    public void CancelTimer(string roomId)
    {
        if (!_roomStates.TryGetValue(roomId, out var state))
        {
            return;
        }

        if (state.TimerCts != null)
        {
            state.TimerCts.Cancel();
            state.TimerCts.Dispose();
            state.TimerCts = null;
        }

        state.TimerEnd = null;
    }

    private async Task RunTimerAsync(string roomId, ArcadeRoomState state, int minutes,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromMinutes(minutes), cancellationToken);

            var participantNames = ResolveParticipantNames(roomId, state);
            var title = state.Title;
            string html;

            if (participantNames.Count > 0)
            {
                var participantList = string.Join(", ", participantNames);
                html =
                    $"<b>🏆 {title} - Timer terminé !</b><br><b>Participants ({participantNames.Count}) :</b> {participantList}";
            }
            else
            {
                html = $"<b>⏰ {title} - Timer terminé</b><br>Aucun participant inscrit.";
            }

            state.IsTimerExpired = true;
            _bot.Say(roomId, $"/addhtmlbox {html}");
        }
        catch (OperationCanceledException)
        {
            // Timer was cancelled, silently ignore
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error in arcade timer for room {RoomId}", roomId);
        }
    }

    private List<string> ResolveParticipantNames(string roomId, ArcadeRoomState state)
    {
        var participantNames = new List<string>();

        if (!_roomsManager.HasRoom(roomId))
        {
            participantNames.AddRange(state.Participants);
            return participantNames;
        }

        var room = _roomsManager.GetRoom(roomId);
        foreach (var participantId in state.Participants)
        {
            if (room.Users.TryGetValue(participantId, out var user))
            {
                participantNames.Add(user.Name);
            }
            else
            {
                participantNames.Add(participantId);
            }
        }

        return participantNames;
    }
}