using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Models;

public interface IGame
{
    bool HasBeenCancelled { get; }
    IContext Context { get; set; }
    string Identifier { get; }
    void Cancel();
}