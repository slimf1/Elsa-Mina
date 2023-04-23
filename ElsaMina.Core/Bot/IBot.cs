namespace ElsaMina.Core.Bot;

public interface IBot : IDisposable
{
    Task Start();
}