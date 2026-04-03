using System.Collections.Concurrent;
using ElsaMina.Core.Services.Clock;

namespace ElsaMina.Commands.Arcade.Events;

public class ArcadeEventsService : IArcadeEventsService
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _muteExpirationDate = new();
    private readonly IClockService _clockService;

    public ArcadeEventsService(IClockService clockService)
    {
        _clockService = clockService;
    }

    public void MuteGames(string roomId, TimeSpan duration)
    {
        _muteExpirationDate[roomId] = _clockService.CurrentUtcDateTimeOffset.Add(duration);
    }

    public void UnmuteGames(string roomId)
    {
        _muteExpirationDate.TryRemove(roomId, out _);
    }

    public bool AreGamesMuted(string roomId)
    {
        return _muteExpirationDate.TryGetValue(roomId, out var expiry)
               && _clockService.CurrentUtcDateTimeOffset < expiry;
    }
}
