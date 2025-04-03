namespace ElsaMina.Core.Services.Rooms;

public interface IGame
{
    event Action GameStarted;
    event Action GameEnded;
    string Identifier { get; }
    bool IsStarted { get; }
}