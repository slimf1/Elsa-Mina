namespace ElsaMina.Core.Services.System;

public class SystemService : ISystemService
{
    public void Sleep(TimeSpan delay) => Thread.Sleep(delay);
    public Task SleepAsync(TimeSpan delay) => Task.Delay(delay);
    public void Kill(int code = 1) => Environment.Exit(code);
}