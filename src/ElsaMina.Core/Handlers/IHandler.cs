namespace ElsaMina.Core.Handlers;

public interface IHandler
{
    bool IsEnabled { get; set; }
    string Identifier { get; }

    Task OnInitialize();
    Task OnMessageReceived(string[] parts, string roomId = null);
}