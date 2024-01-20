namespace ElsaMina.Core.Services.System;

public interface ISystemService
{
    void Sleep(int millis);
    Task SleepAsync(int millis);
    void Kill(int code = 1);
}