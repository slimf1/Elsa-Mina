namespace ElsaMina.Core.Services.System;

public class SystemService : ISystemService
{
    public void Sleep(TimeSpan delay) => Thread.Sleep(delay);
    public Task SleepAsync(TimeSpan delay, CancellationToken cancellationToken = default) => Task.Delay(delay, cancellationToken);
    public void Kill(int code = 1) => Environment.Exit(code);
}