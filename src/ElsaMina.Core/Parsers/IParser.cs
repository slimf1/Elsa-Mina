namespace ElsaMina.Core.Parsers;

public interface IParser
{
    string Identifier { get; }
    bool IsEnabled { get; set; }

    Task OnInitialize();
    Task Invoke(string[] parts, string roomId = null);
}