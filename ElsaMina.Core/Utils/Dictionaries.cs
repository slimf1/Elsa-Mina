namespace ElsaMina.Core.Utils;

public static class Dictionaries
{
    public static IDictionary<string, object> ParseArguments(this string argumentString)
    {
        return argumentString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('='))
            .ToDictionary(split => split[0], split => (object)split[1]);
    }
}