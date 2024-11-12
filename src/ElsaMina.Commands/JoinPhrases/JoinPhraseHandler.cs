using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.JoinPhrases;

public class JoinPhraseHandler : Handler
{
    private static readonly TimeSpan JOIN_PHRASE_COOLDOWN = TimeSpan.FromHours(3);

    private readonly IRoomUserDataService _roomUserDataService;
    private readonly IBot _bot;
    private readonly IClockService _clockService;

    private readonly Dictionary<Tuple<string, string>, DateTime> _lastJoinPhraseTrigger = new();

    public JoinPhraseHandler(IRoomUserDataService roomUserDataService,
        IBot bot,
        IClockService clockService)
    {
        _roomUserDataService = roomUserDataService;
        _bot = bot;
        _clockService = clockService;
    }

    public override Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (parts.Length != 3 || parts[1] != "J")
        {
            return Task.CompletedTask;
        }

        var userId = parts[2].ToLowerAlphaNum();
        var key = new Tuple<string, string>(userId, roomId);

        if (!_roomUserDataService.JoinPhrases.TryGetValue(key, out var joinPhrase))
        {
            return Task.CompletedTask;
        }

        var now = _clockService.CurrentUtcDateTime;

        _lastJoinPhraseTrigger.TryGetValue(key, out var lastTrigger);
        if (now - lastTrigger < JOIN_PHRASE_COOLDOWN)
        {
            return Task.CompletedTask;
        }

        _lastJoinPhraseTrigger[key] = now;
        _bot.Say(roomId, joinPhrase);

        return Task.CompletedTask;
    }
}