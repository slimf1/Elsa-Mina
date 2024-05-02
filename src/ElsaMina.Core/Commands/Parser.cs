namespace ElsaMina.Core.Commands;

public abstract class Parser : IParser
{
    public abstract string Identifier { get; }
    public bool IsEnabled { get; set; } = true;

    public async Task Invoke(string[] parts, string roomId = null)
    {
        try
        {
            await Execute(parts, roomId);
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "An error occured while executing parser '{0}'", Identifier);
        }
    }

    protected abstract Task Execute(string[] parts, string roomId = null);
}