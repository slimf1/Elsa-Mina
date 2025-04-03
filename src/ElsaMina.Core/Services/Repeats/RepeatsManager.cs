using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Repeats;

public class RepeatsManager : IRepeatsManager
{
    private readonly HashSet<Repeat> _repeats = new();

    public bool StartRepeat(IContext context, string message, TimeSpan interval)
    {
        var repeat = new Repeat(context, Guid.NewGuid(), context.RoomId, message, interval);
        if (!_repeats.Add(repeat))
        {
            return false;
        }

        repeat.Start();
        return true;
    }

    public IRepeat GetRepeat(string roomId, Guid repeatId)
    {
        return _repeats.FirstOrDefault(repeat => repeat.RepeatId == repeatId
                                                 && repeat.RoomId == roomId);
    }

    public IEnumerable<IRepeat> GetRepeats(string roomId)
    {
        return _repeats.Where(repeat => repeat.RoomId == roomId);
    }

    public bool StopRepeat(string roomId, Guid repeatId)
    {
        if (GetRepeat(roomId, repeatId) is not Repeat repeat)
        {
            return false;
        }

        repeat.Stop();
        _repeats.Remove(repeat);
        return true;
    }
}