using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElsaMina.Commands.Misc.Dictionary;

[JsonConverter(typeof(DictionaryApiResponseConverter))]
public class DictionaryApiResponse
{
    public List<DictionaryApiEntry> Entries { get; init; }
    public List<string> Suggestions { get; init; }

    public bool HasSuggestions => Suggestions is { Count: > 0 };
    public bool IsEmpty => (Entries == null || Entries.Count == 0) && !HasSuggestions;
}

public class DictionaryApiResponseConverter : JsonConverter<DictionaryApiResponse>
{
    public override DictionaryApiResponse ReadJson(JsonReader reader, Type objectType,
        DictionaryApiResponse existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        if (token.Type != JTokenType.Array || !token.HasValues)
            return new DictionaryApiResponse();

        if (token.First!.Type == JTokenType.String)
            return new DictionaryApiResponse { Suggestions = token.ToObject<List<string>>(serializer) };

        return new DictionaryApiResponse { Entries = token.ToObject<List<DictionaryApiEntry>>(serializer) };
    }

    public override void WriteJson(JsonWriter writer, DictionaryApiResponse value, JsonSerializer serializer)
        => throw new NotSupportedException();
}
