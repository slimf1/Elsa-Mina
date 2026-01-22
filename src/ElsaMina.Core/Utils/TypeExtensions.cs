using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Core.Utils;

public static class TypeExtensions
{
    public static NamedCommandAttribute GetCommandAttribute(this Type type)
    {
        return type.GetCustomAttributes(typeof(NamedCommandAttribute), false).FirstOrDefault() as NamedCommandAttribute;
    }
}