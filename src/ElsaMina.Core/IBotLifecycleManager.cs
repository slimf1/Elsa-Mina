namespace ElsaMina.Core;

public interface IBotLifecycleManager
{
    void OnStart();
    void OnReconnect();
    void OnDisconnect();
}