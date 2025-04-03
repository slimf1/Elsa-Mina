using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public interface IGame
{
    event Action GameStarted;
    event Action GameEnded;
    string Identifier { get; }
    bool IsStarted { get; }
}