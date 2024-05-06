namespace ElsaMina.Core.Parsers;

public interface IParsersManager
{
    bool IsInitialized { get; }
    Task Initialize();
    Task Parse(string[] parts, string roomId = null);
}