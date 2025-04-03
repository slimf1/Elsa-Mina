namespace ElsaMina.Core.Services.Rooms;

public abstract class Game : IGame
{
    public event Action GameStarted = delegate { };
    public event Action GameEnded = delegate { };

    public bool IsStarted { get; private set; }
    public bool IsEnded { get; private set; }
    public abstract string Identifier { get; }

    protected void OnStart()
    {
        if (IsStarted)
        {
            return;
        }
        IsStarted = true;
        IsEnded = false;
        GameStarted();
    }

    protected void OnEnd()
    {
        if (IsEnded)
        {
            return;
        }
        IsStarted = false;
        IsEnded = true;
        GameEnded();
    }
}