namespace ElsaMina.Core.Parsers;

public interface IParsersManager
{
    bool IsInitialized { get; }
    void Initialize();
    Task Parse(string[] parts, string roomId = null);
}