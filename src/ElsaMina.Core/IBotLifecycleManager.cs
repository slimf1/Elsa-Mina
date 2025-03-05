namespace ElsaMina.Core;

public interface IBotLifecycleManager
{
    void OnConnect();
    void OnStart();
    void OnReset();
}