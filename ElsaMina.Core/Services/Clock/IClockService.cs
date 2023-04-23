namespace ElsaMina.Core.Services.Clock;

public interface IClockService
{
    DateTimeOffset Now { get; }
}