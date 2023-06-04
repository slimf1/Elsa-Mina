using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public abstract class Game : IGame
{
    protected Game(IContext context)
    {
        Context = context;
    }

    public IContext Context { get; init; }
}