using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public interface IGame
{
    IContext Context { get; set; }
    void Cancel();
}