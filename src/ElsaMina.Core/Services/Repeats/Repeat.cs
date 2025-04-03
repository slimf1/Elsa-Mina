using System.Timers;
using ElsaMina.Core.Contexts;
using Timer = System.Timers.Timer;

namespace ElsaMina.Core.Services.Repeats;

public sealed class Repeat : IEquatable<Repeat>, IRepeat
{
    private readonly IContext _context;
    private Timer _timer;

    public Repeat(IContext context, Guid repeatId, string roomId, string message, TimeSpan interval)
    {
        _context = context;
        RepeatId = repeatId;
        RoomId = roomId;
        Message = message;
        Interval = interval;
    }

    public string RoomId { get; }
    public Guid RepeatId { get; }
    public string Message { get; }
    public TimeSpan Interval { get; }

    public void Start()
    {
        CancelTimer();
        _timer = new Timer(Interval);
        _timer.Elapsed += HandleTimerElapsed;
        _timer.Start();
    }

    public void Stop()
    {
        CancelTimer();
    }

    private void CancelTimer()
    {
        if (_timer == null)
        {
            return;
        }

        _timer.Elapsed -= HandleTimerElapsed;
        _timer.Dispose();
        _timer = null;
    }

    private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
    {
        var prefix = Message.StartsWith("/wall") || Message.StartsWith("/announce") ? string.Empty : "[[]]";
        _context.Reply($"{prefix}{Message}");
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RoomId, Message);
    }

    public bool Equals(Repeat other)
    {
        return other != null && other.RoomId == RoomId && other.Message == Message;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Repeat);
    }
}