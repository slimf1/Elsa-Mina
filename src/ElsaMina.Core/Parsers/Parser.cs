namespace ElsaMina.Core.Parsers;

public abstract class Parser : IParser
{
    public abstract string Identifier { get; }
    public bool IsEnabled { get; set; } = true;

    public virtual Task OnInitialize()
    {
        return Task.CompletedTask;
    }

    public async Task Invoke(string[] parts, string roomId = null)
    {
        try
        {
            await Execute(parts, roomId);
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "An error occurred while executing parser '{0}'", Identifier);
            throw;
        }
    }

    protected abstract Task Execute(string[] parts, string roomId = null);
}