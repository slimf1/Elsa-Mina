namespace ElsaMina.Core.Services.Parsers;

public interface IParsersManager
{
    bool IsInitialized { get; }
    void Initialize();
    Task Parse(string[] parts);
}