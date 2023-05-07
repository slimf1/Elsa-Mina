namespace ElsaMina.Core.Services.Clock;

public class ClockService : IClockService
{
    public DateTimeOffset CurrentDateTimeOffset => DateTimeOffset.Now;
    public DateTime CurrentDateTime => DateTime.Now;
}