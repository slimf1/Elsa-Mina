namespace ElsaMina.Core;

public interface IBotLifecycleHandler
{
    void OnStart();
    void OnReconnect();
    void OnDisconnect();
}