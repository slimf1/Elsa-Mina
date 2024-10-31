using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public abstract class Game : IGame
{
    public bool HasBeenCancelled { get; private set; }
    
    public IContext Context { get; set; }

    public Action CleanupAction { get; set; }

    public abstract string Identifier { get; }
    
    public virtual void Cancel() // todo : find a better way to handle lifecycle
    {
        if (HasBeenCancelled)
        {
            return;
        }
        HasBeenCancelled = true;
        CleanupAction?.Invoke();
    }
}