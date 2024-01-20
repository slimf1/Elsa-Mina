#nullable enable
using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Repeats;

public class RepeatsManager : IRepeatsManager
{
    private readonly HashSet<Repeat> _repeats = new();

    public Repeat? StartRepeat(IContext context, string repeatId, string message, uint intervalInMinutes)
    {
        var repeat = new Repeat
        {
            RoomId = context.RoomId,
            Interval = intervalInMinutes,
            RepeatId = repeatId
        };

        if (_repeats.Contains(repeat))
        {
            return null;
        }

        var prefix = message.StartsWith("/wall") || message.StartsWith("/announce") ? string.Empty : "[[]]";
        var timer = new Timer(_ =>
        {
            // Prevent command injection
            context.Reply($"{prefix}{message}");
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(intervalInMinutes));
        repeat.Timer = timer;

        _repeats.Add(repeat);

        return repeat;
    }

    public Repeat? GetRepeat(string roomId, string repeatId)
    {
        return _repeats.FirstOrDefault(repeat => repeat.RepeatId == repeatId
                                                 && repeat.RoomId == roomId);
    }

    public IEnumerable<Repeat> GetRepeatsFromRoom(string roomId)
    {
        return _repeats.Where(repeat => repeat.RoomId == roomId);
    }

    public bool StopRepeat(string roomId, string timerId)
    {
        var repeat = GetRepeat(roomId, timerId);
        if (repeat == null)
        {
            return false;
        }

        repeat.Timer.Dispose();
        _repeats.Remove(repeat);
        return true;
    }
}