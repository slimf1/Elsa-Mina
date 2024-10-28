using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public abstract class Game : IGame
{
    public IContext Context { get; set; }

    public Action CleanupAction { get; set; }

    public abstract string Identifier { get; }
    
    public virtual void Cancel()
    {
        CleanupAction?.Invoke();
    }
}