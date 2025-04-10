namespace ElsaMina.Core.Handlers;

public abstract class Handler : IHandler
{
    public bool IsEnabled { get; set; } = true;
    public string Identifier => GetType().FullName;
    public virtual int Priority => 0;

    public abstract Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default);

    public virtual void OnStart()
    {
    }

    public virtual void OnReconnect()
    {
    }

    public virtual void OnDisconnect()
    {
    }
}