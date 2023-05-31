using System.Dynamic;

namespace ElsaMina.Core.Utils;

public static class Dictionaries
{
    public static IDictionary<string, object> ParseArguments(this string argumentString)
    {
        return argumentString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('='))
            .ToDictionary(split => split[0], split => (object)split[1]);
    }

    public static object ToAnonymousObject(this IDictionary<string, object> dictionary)
    {
        var expandoObject = new ExpandoObject();
        var collection = (ICollection<KeyValuePair<string, object>>)expandoObject;

        foreach (var kvp in dictionary)
        {
            collection.Add(kvp);
        }

        dynamic expandoObjectDynamic = collection;
        return expandoObjectDynamic;
    }
}