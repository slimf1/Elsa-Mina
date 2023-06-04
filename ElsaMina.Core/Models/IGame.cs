using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public interface IGame
{
    protected IContext Context { get; init; }
}