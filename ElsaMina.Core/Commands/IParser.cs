namespace ElsaMina.Core.Commands;

public interface IParser
{
    bool IsEnabled { get; set; }
    Task Execute(string[] parts, string roomId = null);
}