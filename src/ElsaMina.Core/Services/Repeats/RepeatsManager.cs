using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Repeats;

public class RepeatsManager : IRepeatsManager
{
    private readonly Dictionary<Guid, Repeat> _repeats = new();

    public void StartRepeat(IContext context, string roomId, string message, TimeSpan interval)
    {
        var repeat = new Repeat(context, Guid.NewGuid(), roomId, message, interval);
        _repeats[repeat.RepeatId] = repeat;
        repeat.Start();
    }

    public IRepeat GetRepeat(Guid repeatId)
    {
        return _repeats.GetValueOrDefault(repeatId);
    }

    public IEnumerable<IRepeat> GetRepeats(string roomId)
    {
        return _repeats.Values.Where(repeat => repeat.RoomId == roomId);
    }

    public bool StopRepeat(Guid repeatId)
    {
        if (!_repeats.TryGetValue(repeatId, out var repeat))
        {
            return false;
        }

        repeat.Stop();
        _repeats.Remove(repeatId);
        return true;
    }
}