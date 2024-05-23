namespace ElsaMina.Core.Services.System;

public interface ISystemService
{
    void Sleep(TimeSpan delay);
    Task SleepAsync(TimeSpan delay);
    void Kill(int code = 1);
}