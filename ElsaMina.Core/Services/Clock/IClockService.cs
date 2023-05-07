namespace ElsaMina.Core.Services.Clock;

public interface IClockService
{
    DateTimeOffset CurrentDateTimeOffset { get; }
    DateTime CurrentDateTime { get; }
}