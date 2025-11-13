using ElsaMina.Core.Services.Http;
using ElsaMina.Logging;

namespace ElsaMina.Core.Services.Dex;

public class DexManager : IDexManager
{
    public const string DEX_URL = "https://tyradex.vercel.app/api/v1/pokemon";

    private readonly IHttpService _httpService;

    public DexManager(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public IReadOnlyList<Pokemon> Pokedex { get; private set; } = [];

    public async Task LoadDexAsync(CancellationToken cancellationToken = default)
    {
        try
        {
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