using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public abstract class Game : IGame
{
    public IContext Context { get; set; }

    public virtual void Cancel()
    {
    }
}