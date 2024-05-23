namespace ElsaMina.Core.Handlers;

public interface IHandler
{
    string Identifier { get; }
    bool IsEnabled { get; set; }

    Task OnInitialize();
    Task Invoke(string[] parts, string roomId = null);
}