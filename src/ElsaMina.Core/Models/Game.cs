﻿using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public abstract class Game : IGame
{
    public event Action GameStarted = delegate { };
    public event Action GameEnded = delegate { };

    public IContext Context { get; set; }
    public bool IsStarted { get; private set; }
    public bool IsEnded { get; private set; }
    public abstract string Identifier { get; }

    protected void OnStart()
    {
        IsStarted = true;
        IsEnded = false;
        GameStarted();
    }

    protected void OnEnd()
    {
        IsStarted = false;
        IsEnded = true;
        GameEnded();
    }
}