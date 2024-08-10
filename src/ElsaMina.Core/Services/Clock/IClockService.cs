namespace ElsaMina.Core.Services.Clock;

public interface IClockService
{
    DateTime CurrentUtcDateTime { get; }
}