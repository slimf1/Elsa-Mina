namespace ElsaMina.Core.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class NamedCommandAttribute : Attribute
{
    public NamedCommandAttribute(string name)
    {
        Name = name;
    }

    public NamedCommandAttribute(string name, params string[] aliases)
    {
        Name = name;
        Aliases = aliases;
    }

    public string Name { get; }

    public string[] Aliases { get; set; } = [];
}