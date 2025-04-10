namespace ElsaMina.Core.Handlers;

public interface IHandler : IBotLifecycleHandler
{
    bool IsEnabled { get; set; }
    string Identifier { get; }

    Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default);
}