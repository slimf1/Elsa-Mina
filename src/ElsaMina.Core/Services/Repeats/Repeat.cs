namespace ElsaMina.Core.Services.Repeats;

public class Repeat : IEquatable<Repeat>
{
    public string RoomId { get; init; }
    public string RepeatId { get; init; }
    public string Message { get; init; }
    public uint Interval { get; init; }
    public Timer Timer { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as Repeat);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RoomId, RepeatId);
    }

    public bool Equals(Repeat other)
    {
        return other != null && other.RoomId == RoomId && other.RepeatId == RepeatId;
    }
}