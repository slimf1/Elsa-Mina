namespace ElsaMina.Core.Services.Repeats;

public interface IRepeat
{
    string RoomId { get; }
    Guid RepeatId { get; }
    string Message { get; }
    TimeSpan Interval { get; }
}