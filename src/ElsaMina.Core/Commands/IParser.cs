namespace ElsaMina.Core.Commands;

public interface IParser
{
    string Identifier { get; }
    bool IsEnabled { get; set; }

    Task Invoke(string[] parts, string roomId = null);
}