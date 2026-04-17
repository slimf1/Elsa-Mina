namespace ElsaMina.Core.Services.Games;

public interface IGame
{
    event Action GameStarted;
    event Action GameEnded;
    string Identifier { get; }
    bool IsStarted { get; }
    bool IsEnded { get; }
}