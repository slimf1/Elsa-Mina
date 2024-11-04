using ElsaMina.Core.Services.Http;

namespace ElsaMina.Core.Services.Dex;

public class DexManager : IDexManager
{
    private const string DEX_URL = "https://tyradex.tech/api/v1/pokemon";

    private readonly IHttpService _httpService;

    public DexManager(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public IReadOnlyList<Pokemon> Pokedex { get; private set; } = [];

    public async Task LoadDex()
    {
        try
        {
            var response = await _httpService.Get<List<Pokemon>>(DEX_URL);
            Pokedex = response.Data;
            Logger.Information("Dex : Fetched {0} entries", Pokedex.Count);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error loading Dex");
        }
    }
}