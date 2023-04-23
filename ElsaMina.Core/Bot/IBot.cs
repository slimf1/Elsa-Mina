using ElsaMina.Core.Models;

namespace ElsaMina.Core.Bot;

public interface IBot : IDisposable
{
    IDictionary<string, IRoom> Rooms { get; }
    IEnumerable<string> Formats { get; }
    Task HandleReceivedMessage(string message);

    Task Start();
    void Send(string message);
    void Say(string roomId, string message);
}