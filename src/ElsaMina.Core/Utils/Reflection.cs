using ElsaMina.Core.Commands;

namespace ElsaMina.Core.Utils;

public static class Reflection
{
    public static NamedCommandAttribute GetCommandAttribute(this Type type)
    {
        return type.GetCustomAttributes(typeof(NamedCommandAttribute), false).FirstOrDefault() as NamedCommandAttribute;
    }
}