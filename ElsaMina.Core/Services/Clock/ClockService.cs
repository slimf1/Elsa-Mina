namespace ElsaMina.Core.Services.Clock;

public class ClockService : IClockService
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}