namespace ElsaMina.Core.Services.System;

public class SystemService : ISystemService
{
    public void Sleep(int millis) => Thread.Sleep(millis);
    public Task SleepAsync(int millis) => Task.Delay(millis);
    public void Kill(int code = 1) => Environment.Exit(code);
}