namespace ElsaMina.Core.Services.System;

public class SystemService : ISystemService
{
    public void Sleep(int millis) => Thread.Sleep(millis);
    public void Kill(int code = 0) => Environment.Exit(code);
}