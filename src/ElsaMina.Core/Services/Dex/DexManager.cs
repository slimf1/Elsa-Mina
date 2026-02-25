using ElsaMina.Logging;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Dex;

public class DexManager : IDexManager
{
    public IReadOnlyDictionary<string, PokedexEntry> Pokedex { get; private set; } = new Dictionary<string, PokedexEntry>();
    public IReadOnlyDictionary<string, MoveData> Moves { get; private set; } = new Dictionary<string, MoveData>();
    // todo : abilities & items

    public async Task LoadDexAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Pokedex = await ReadJsonFileAsync<Dictionary<string, PokedexEntry>>("pokedex.json", cancellationToken);
            Moves = await ReadJsonFileAsync<Dictionary<string, MoveData>>("moves.json", cancellationToken);
            Log.Information("Dex: loaded {0} Pokémon entries and {1} moves", Pokedex.Count, Moves.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading Dex");
        }
    }

    private static async Task<T> ReadJsonFileAsync<T>(string filename, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(Path.Join("Services", "Dex", filename));
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync(cancellationToken);
        return JsonConvert.DeserializeObject<T>(json);
    }
}
