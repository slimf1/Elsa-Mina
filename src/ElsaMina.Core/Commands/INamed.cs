namespace ElsaMina.Core.Commands;

public interface INamed
{
    public static virtual string Name => string.Empty;
    public static virtual IEnumerable<string> Aliases => Enumerable.Empty<string>();
}