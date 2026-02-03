using ElsaMina.Core.Services.Http;
using ElsaMina.Logging;
using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Dex;

public class DexManager : IDexManager
{
    public const string DEX_URL = "https://tyradex.vercel.app/api/v1/pokemon";

    private readonly IHttpService _httpService;

    public DexManager(IHttpService httpService)
    {
        _httpService = httpService;
    }

    [Obsolete("use Pokedex2 instead")]
    public IReadOnlyList<Pokemon> Pokedex { get; private set; } = [];

    public IReadOnlyDictionary<string, PokedexEntry> Pokedex2 { get; private set; } = new Dictionary<string, PokedexEntry>();
    public IReadOnlyDictionary<string, MoveData> Moves { get; private set; } = new Dictionary<string, MoveData>();
    // todo : abilities & items

    public async Task LoadDexAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var stream = File.OpenRead(Path.Join("Services", "Dex", "pokedex.json"));
            using var reader = new StreamReader(stream);
    
            var json = await reader.ReadToEndAsync(cancellationToken);
    
            Pokedex2 = JsonConvert.DeserializeObject<Dictionary<string, PokedexEntry>>(json);

            await using var stream2 = File.OpenRead(Path.Join("Services", "Dex", "moves.json"));
            using var reader2 = new StreamReader(stream2);
            var json2 = await reader2.ReadToEndAsync(cancellationToken);
            Moves = JsonConvert.DeserializeObject<Dictionary<string, MoveData>>(json2);
            var response = await _httpService.GetAsync<List<Pokemon>>(DEX_URL, cancellationToken: cancellationToken);
            Pokedex = response.Data;
            Log.Information("Dex : Fetched {0} entries", Pokedex.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading Dex");
        }
    }
}