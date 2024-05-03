namespace ElsaMina.Core.Contexts;

public interface IContextFactory
{
    IContext TryBuildContextFromReceivedMessage(string[] parts, string roomId = null);
}