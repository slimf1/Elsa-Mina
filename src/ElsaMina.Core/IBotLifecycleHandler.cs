namespace ElsaMina.Core;

public interface IBotLifecycleHandler
{
    void OnConnect();
    void OnStart();
    void OnReset();
}