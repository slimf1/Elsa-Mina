namespace ElsaMina.Core.Services.Clock;

public class ClockService : IClockService
{
    public DateTime CurrentUtcDateTime => DateTime.UtcNow;
    public DateTimeOffset CurrentUtcDateTimeOffset => DateTimeOffset.UtcNow;
}