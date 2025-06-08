namespace ElsaMina.Core.Handlers;

public abstract class Handler : IHandler
{
    public bool IsEnabled { get; set; } = true;
    public string Identifier => GetType().FullName;

    public abstract Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default);
}