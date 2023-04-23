namespace ElsaMina.Core.Commands;

public abstract class ICommand
{
    public abstract string Name { get; }
    public virtual IEnumerable<string> Aliases { get; }
}