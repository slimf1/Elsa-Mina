namespace ElsaMina.Core.Handlers;

public abstract class Handler : IHandler
{
    public abstract string Identifier { get; }
    public bool IsEnabled { get; set; } = true;

    public virtual Task OnInitialize()
    {
        return Task.CompletedTask;
    }

    public async Task OnMessageReceived(string[] parts, string roomId = null)
    {
        try
        {
            await HandleReceivedMessage(parts, roomId);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "An error occurred while executing handler '{0}'", Identifier);
            throw;
        }
    }

    public abstract Task HandleReceivedMessage(string[] parts, string roomId = null);
}