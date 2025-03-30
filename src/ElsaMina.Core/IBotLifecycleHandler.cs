namespace ElsaMina.Core;

public interface IBotLifecycleHandler
{
    int Priority { get; }
    void OnStart();
    void OnReconnect();
    void OnDisconnect();
}