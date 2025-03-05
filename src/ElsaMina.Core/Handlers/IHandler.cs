namespace ElsaMina.Core.Handlers;

public interface IHandler : IBotLifecycleHandler
{
    bool IsEnabled { get; set; }
    string Identifier { get; }

    Task OnMessageReceived(string[] parts, string roomId = null);
}