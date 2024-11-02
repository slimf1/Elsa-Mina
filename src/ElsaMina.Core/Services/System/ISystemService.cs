namespace ElsaMina.Core.Services.System;

public interface ISystemService
{
    void Sleep(TimeSpan delay);
    Task SleepAsync(TimeSpan delay, CancellationToken cancellationToken = default);
    void Kill(int code = 1);
}