namespace ElsaMina.Core.Services.System;

public interface ISystemService
{
    void Sleep(int millis);
    void Kill(int code = 0);
}