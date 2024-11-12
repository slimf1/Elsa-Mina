namespace ElsaMina.Core.Handlers;

public interface IHandler
{
    bool IsEnabled { get; set; }

    Task OnInitialize();
    Task OnMessageReceived(string[] parts, string roomId = null);
}