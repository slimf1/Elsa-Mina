namespace ElsaMina.Core.Handlers;

public abstract class Handler : IHandler
{
    public bool IsEnabled { get; set; } = true;
    public string Identifier => GetType().FullName;
    public virtual int Priority => 0;

    public async Task OnMessageReceivedAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await HandleReceivedMessageAsync(parts, roomId, cancellationToken);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred while executing handler '{0}'", Identifier);
            throw;
        }
    }

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